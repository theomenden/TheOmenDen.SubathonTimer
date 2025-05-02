
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheOmenDen.TwitchInterop.Extensions;
using TheOmenDen.TwitchInterop.Models;
using TheOmenDen.TwitchInterop.Services;

namespace TheOmenDen.TwitchInterop
{
    public class TwitchFunctionContainer
    {
        private readonly ILogger<TwitchFunctionContainer> _logger;
        private readonly TwitchSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITwitchUserMappingService _mappingService;
        public TwitchFunctionContainer(
            ILogger<TwitchFunctionContainer> logger,
            IOptions<TwitchSettings> settings,
            ITwitchUserMappingService mappingService,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _settings = settings.Value;
            _mappingService = mappingService;
            _httpClientFactory = httpClientFactory;
        }

        [Function(nameof(TwitchOAuthCallback))]
        public async Task<HttpResponseData> TwitchOAuthCallback(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "twitch/oauth/callback")] HttpRequestData req,
    FunctionContext context,
    CancellationToken cancellationToken)
        {
            var logger = context.GetLogger(nameof(TwitchOAuthCallback));
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

            var code = query["code"];
            var state = query["state"]; // Optional

            if (string.IsNullOrWhiteSpace(code))
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Missing authorization code", cancellationToken);
                return bad;
            }

            var redirectUri = "https://<your-function-app>.azurewebsites.net/api/twitch/oauth/callback";

            var oauthClient = _httpClientFactory.CreateClient(TwitchConstants.TwitchOAuthHttpClientName);

            var form = new Dictionary<string, string>
            {
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret,
                ["code"] = code,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = redirectUri
            };

            var response = await oauthClient.PostAsync("", new FormUrlEncodedContent(form), cancellationToken);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Token exchange failed: {StatusCode}, {Content}", response.StatusCode, json);
                var fail = req.CreateResponse(HttpStatusCode.InternalServerError);
                await fail.WriteStringAsync("OAuth token exchange failed", cancellationToken);
                return fail;
            }

            var tokenData = JsonSerializer.Deserialize<TwitchOAuthTokenResponse>(json);
            if (tokenData is null)
            {
                logger.LogError("Failed to parse token response.");
                var fail = req.CreateResponse(HttpStatusCode.InternalServerError);
                await fail.WriteStringAsync("Token response was invalid", cancellationToken);
                return fail;
            }

            // Use token to fetch Twitch user info
            var twitchClient = _httpClientFactory.CreateClient(TwitchConstants.TwitchHttpClientName);
            twitchClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TwitchConstants.AuthHeaderScheme, tokenData.AccessToken);
            twitchClient.DefaultRequestHeaders.Add(TwitchConstants.ClientIdHeader, _settings.ClientId);

            var userResponse = await twitchClient.GetAsync(TwitchConstants.Endpoints.Users, cancellationToken);
            var userJson = await userResponse.Content.ReadAsStringAsync(cancellationToken);

            if (!userResponse.IsSuccessStatusCode)
            {
                logger.LogError("User lookup failed: {StatusCode}, {Content}", userResponse.StatusCode, userJson);
                var fail = req.CreateResponse(HttpStatusCode.BadGateway);
                await fail.WriteStringAsync("Twitch user lookup failed", cancellationToken);
                return fail;
            }

            var userInfo = JsonSerializer.Deserialize<TwitchUserResponse>(userJson);
            var user = userInfo?.Data?.FirstOrDefault();

            if (user is null)
            {
                var fail = req.CreateResponse(HttpStatusCode.BadRequest);
                await fail.WriteStringAsync("Unable to extract user information", cancellationToken);
                return fail;
            }

            // Get Azure AD user ID from context
            var azureUserId = context.GetUserId();

            // Store the mapping securely
            await _mappingService.StoreMappingAsync(user.Id, azureUserId, cancellationToken);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteStringAsync($"Linked Twitch ID {user.Id} with Azure ID {azureUserId}", cancellationToken);
            return ok;
        }


        [Function(nameof(GetTwitchUserInfo))]
        public async Task<HttpResponseData> GetTwitchUserInfo(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
    FunctionContext context,
    CancellationToken cancellationToken)
        {
            try
            {
                var body = await JsonSerializer.DeserializeAsync<AccessTokenRequest>(req.Body, cancellationToken: cancellationToken);
                if (body is null || string.IsNullOrEmpty(body.AccessToken))
                {
                    _logger.LogWarning("Access token missing");
                    var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                    await bad.WriteStringAsync("Missing access token", cancellationToken);
                    return bad;
                }

                var client = _httpClientFactory.CreateClient(TwitchConstants.TwitchHttpClientName);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TwitchConstants.AuthHeaderScheme, body.AccessToken);
                client.DefaultRequestHeaders.Add(TwitchConstants.ClientIdHeader, _settings.ClientId);

                var response = await client.GetAsync(TwitchConstants.Endpoints.Users, cancellationToken);
                var json = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("User info failed: {StatusCode}, {Body}", response.StatusCode, json);
                    var fail = req.CreateResponse(HttpStatusCode.BadGateway);
                    await fail.WriteStringAsync("Twitch user lookup failed", cancellationToken);
                    return fail;
                }

                var userResponse = JsonSerializer.Deserialize<TwitchUserResponse>(json);

                if (userResponse?.Data?.Length != 1)
                {
                    var fail = req.CreateResponse(HttpStatusCode.BadRequest);
                    await fail.WriteStringAsync("Invalid user response", cancellationToken);
                    return fail;
                }

                var ok = req.CreateResponse(HttpStatusCode.OK);
                await ok.WriteAsJsonAsync(userResponse.Data[0], cancellationToken);
                return ok;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTwitchUserInfo");
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteStringAsync("Internal error", cancellationToken);
                return error;
            }
        }

        [Function(nameof(SubscribeToTwitchEvents))]
        public async Task<HttpResponseData> SubscribeToTwitchEvents(
    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
    FunctionContext context,
    CancellationToken cancellationToken)
        {
            var logger = context.GetLogger(nameof(SubscribeToTwitchEvents));

            var request = await JsonSerializer.DeserializeAsync<TwitchEventSubSubscriptionRequest>(
                req.Body, cancellationToken: cancellationToken);

            if (request is null || string.IsNullOrWhiteSpace(request.AccessToken) || string.IsNullOrWhiteSpace(request.UserId))
            {
                logger.LogWarning("Invalid subscription request payload.");
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Missing access_token or user_id", cancellationToken);
                return bad;
            }

            var callbackUrl = "<YOUR_PUBLIC_EVENTSUB_CALLBACK>"; // Ideally from config

            var client = _httpClientFactory.CreateClient(TwitchConstants.TwitchHttpClientName);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TwitchConstants.AuthHeaderScheme, request.AccessToken);

            client.DefaultRequestHeaders.Add(TwitchConstants.ClientIdHeader, _settings.ClientId);

            var eventTypes = new[]
            {
        TwitchConstants.EventSubTypes.ChannelFollow,
        TwitchConstants.EventSubTypes.ChannelSubscribe,
        TwitchConstants.EventSubTypes.ChannelCheer
    };

            var results = new List<string>();

            foreach (var eventType in eventTypes)
            {
                var payload = new
                {
                    type = eventType,
                    version = TwitchConstants.EventSubVersions.V1,
                    condition = new { broadcaster_user_id = request.UserId },
                    transport = new
                    {
                        method = "webhook",
                        callback = callbackUrl,
                        secret = _settings.SigningSecret
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, MediaTypeNames.Application.Json);
                var response = await client.PostAsync(TwitchConstants.Endpoints.EventSubSubscriptions, content, cancellationToken);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Subscribed to {EventType}", eventType);
                    results.Add($"Subscribed: {eventType}");
                }
                else
                {
                    logger.LogWarning("Failed to subscribe to {EventType} – {StatusCode} – {Body}",
                        eventType, response.StatusCode, responseBody);
                    results.Add($"Failed: {eventType} ({response.StatusCode})");
                }
            }

            var result = req.CreateResponse(HttpStatusCode.OK);
            await result.WriteAsJsonAsync(results, cancellationToken);
            return result;
        }


        [Function(nameof(HandleEventSubWebhook))]
        [SignalROutput(HubName = "twitchhub")]
        public async Task<SignalRMessageAction[]> HandleEventSubWebhook(
     [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options")] HttpRequestData req,
     FunctionContext context,
     CancellationToken cancellationToken)
        {
            var logger = context.GetLogger(nameof(HandleEventSubWebhook));
            var messages = new List<SignalRMessageAction>();

            var body = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);

            // Read headers
            var headers = req.Headers;
            var messageId = GetHeader(headers, TwitchConstants.Headers.TwitchMessageId);
            var timestamp = GetHeader(headers, TwitchConstants.Headers.TwitchMessageTimestamp);
            var signature = GetHeader(headers, TwitchConstants.Headers.TwitchMessageSignature);
            var messageType = GetHeader(headers, TwitchConstants.Headers.TwitchMessageType);

            if (String.IsNullOrWhiteSpace(messageId) ||
                String.IsNullOrWhiteSpace(timestamp) ||
                String.IsNullOrWhiteSpace(signature) ||
                String.IsNullOrWhiteSpace(messageType))
            {
                logger.LogWarning("Missing required Twitch headers.");
                return messages.ToArray(); // Empty = No SignalR output
            }

            // Verify signature using shared SigningKey from config
            var isValid = IsSignatureValid(messageId, timestamp, body, signature, _settings.SigningSecret);
            if (!isValid)
            {
                logger.LogWarning("Invalid Twitch signature.");
                return messages.ToArray();
            }

            var parsed = JsonDocument.Parse(body);
            var root = parsed.RootElement;

            // Handle webhook verification
            if (String.Equals(messageType, "webhook_callback_verification", StringComparison.OrdinalIgnoreCase))
            {
                var challenge = root.GetProperty("challenge").GetString() ?? "";
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync(challenge, cancellationToken);

                // No SignalR message needed
                return messages.ToArray();
            }

            // Handle EventSub notification
            if (String.Equals(messageType, "notification", StringComparison.OrdinalIgnoreCase))
            {
                var eventType = root.GetProperty("subscription").GetProperty("type").GetString() ?? "unknown";
                var broadcasterId = root.GetProperty("subscription").GetProperty("condition").GetProperty("broadcaster_user_id").GetString() ?? "unknown";
                var eventData = root.GetProperty("event").ToString();
                var userId = await _mappingService.GetAzureUserIdAsync(broadcasterId, cancellationToken);
                logger.LogInformation("Received Twitch event: {EventType} for {BroadcasterId}", eventType, broadcasterId);

                messages.Add(new SignalRMessageAction(
                    target: "twitchEvent"
                    )
                {
                    UserId = userId,
                    Arguments = [eventType, eventData]
                });

                return messages.ToArray();
            }

            logger.LogInformation("Unhandled message type: {MessageType}", messageType);
            return messages.ToArray();
        }

        [Function("negotiate")]
        public SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [SignalRConnectionInfoInput(HubName = "twitchhub", UserId = "{headers.x-ms-client-principal-id}")] SignalRConnectionInfo connectionInfo)
        {
            _logger.LogInformation($"SignalR Connection URL = '{connectionInfo.Url}'");
            return connectionInfo;
        }


        private static bool IsSignatureValid(string messageId, string timestamp, string body, string providedSignature, string secret)
        {
            var message = messageId + timestamp + body;
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            var expectedSignature = "sha256=" + BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSignature).AsSpan(),
                Encoding.UTF8.GetBytes(providedSignature).AsSpan()
            );
        }


        private static string? GetHeader(HttpHeadersCollection headers, string key)
        {
            return headers.TryGetValues(key, out var values) ? values.FirstOrDefault() : null;
        }
    }
}
