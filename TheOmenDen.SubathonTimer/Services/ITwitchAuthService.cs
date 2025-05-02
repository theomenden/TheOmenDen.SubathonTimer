using TheOmenDen.SubathonTimer.Models;

namespace TheOmenDen.SubathonTimer.Services;

public interface ITwitchAuthService
{
    Task<bool> ExchangeCodeAsync(string code, string? state, CancellationToken cancellationToken = default);
    Task<TwitchUserInfo?> GetUserInfoAsync(CancellationToken cancellationToken = default);
}