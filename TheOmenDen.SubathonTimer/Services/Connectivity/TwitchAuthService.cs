using System.Net.Http.Json;
using TheOmenDen.SubathonTimer.Models;

namespace TheOmenDen.SubathonTimer.Services.Connectivity;

public sealed class TwitchAuthService(IHttpClientFactory httpClientFactory)
{

    public async Task<bool> ExchangeCodeAsync(string code, string? state, CancellationToken cancellationToken = default)
    {
        var httpClient = httpClientFactory.CreateClient(TwitchConstants.TwitchBackend);

        // Build GET request with query parameters (to match deployed Azure Function)
        var uri = $"twitch/oauth/callback?code={Uri.EscapeDataString(code)}";

        if (!string.IsNullOrWhiteSpace(state))
        {
            uri += $"&state={Uri.EscapeDataString(state)}";
        }

        var response = await httpClient.GetAsync(uri, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<TwitchUserInfo?> GetUserInfoAsync(CancellationToken cancellationToken = default)
    {
        var httpClient = httpClientFactory.CreateClient(TwitchConstants.TwitchBackend);
        return await httpClient.GetFromJsonAsync<TwitchUserInfo>("twitch/userinfo", cancellationToken);
    }
}
