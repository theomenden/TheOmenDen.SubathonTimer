using System.Net.Http.Json;
using TheOmenDen.SubathonTimer.Models;

namespace TheOmenDen.SubathonTimer.Services.Connectivity;

public sealed class TwitchAuthService
{
    private readonly HttpClient _httpClient;

    public TwitchAuthService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient(TwitchConstants.TwitchBackend);
    }

    public async Task<bool> ExchangeCodeAsync(string code, string? state, CancellationToken cancellationToken = default)
    {
        var payload = new { Code = code, State = state };

        var response = await _httpClient.PostAsJsonAsync("twitch/complete", payload, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<TwitchUserInfo?> GetUserInfoAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<TwitchUserInfo>("twitch/userinfo", cancellationToken);
    }
}
