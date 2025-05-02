using System.Net.Http.Json;
using TheOmenDen.SubathonTimer.Models;

namespace TheOmenDen.SubathonTimer.Services.Connectivity;

public sealed class TwitchAuthService(IHttpClientFactory httpClientFactory)
{

    public async Task<bool> ExchangeCodeAsync(string code, string? state, CancellationToken cancellationToken = default)
    {
        var httpClient = httpClientFactory.CreateClient(TwitchConstants.TwitchBackend);
        var payload = new { Code = code, State = state };

        var response = await httpClient.PostAsJsonAsync("twitch/complete", payload, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<TwitchUserInfo?> GetUserInfoAsync(CancellationToken cancellationToken = default)
    {
        var httpClient = httpClientFactory.CreateClient(TwitchConstants.TwitchBackend);
        return await httpClient.GetFromJsonAsync<TwitchUserInfo>("twitch/userinfo", cancellationToken);
    }
}
