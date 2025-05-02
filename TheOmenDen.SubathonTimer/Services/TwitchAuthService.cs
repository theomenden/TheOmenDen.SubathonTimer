using System.Net.Http.Json;
using TheOmenDen.SubathonTimer.Models;

namespace TheOmenDen.SubathonTimer.Services;

public sealed class TwitchAuthService : ITwitchAuthService
{
    private readonly IHttpClientFactory _clientFactory;

    public TwitchAuthService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<bool> ExchangeCodeAsync(string code, string? state, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient(TwitchConstants.TwitchBackend);
        if (string.IsNullOrWhiteSpace(code)) return false;

        // This matches your Azure Function GET endpoint: /twitch/oauth/callback?code=...&state=...
        var uri = $"twitch/oauth/callback?code={Uri.EscapeDataString(code)}";

        if (!string.IsNullOrWhiteSpace(state))
        {
            uri += $"&state={Uri.EscapeDataString(state)}";
        }

        var response = await client.GetAsync(uri, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<TwitchUserInfo?> GetUserInfoAsync(CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient(TwitchConstants.TwitchBackend);
        return await client.GetFromJsonAsync<TwitchUserInfo>("twitch/userinfo", cancellationToken);
    }
}