using System.Text.Json;
using TheOmenDen.SubathonTimer.Models;

namespace TheOmenDen.SubathonTimer.Services;

public interface ITimerService
{
    TimeSpan RemainingTime { get; }
    TimeSpan? DefaultTime { get; }

    void SetTime(TimeSpan time);
    void Reset();
    void ZeroOut();

    void AddManual(TimeSpan time);
    void SubtractManual(TimeSpan time);

    void HandleTwitchEvent(string eventType, JsonElement eventData);

    event Action OnTimerUpdated;

    IReadOnlyCollection<TwitchTimerRule> GetRules();
    void AddRule(TwitchTimerRule rule);
    void RemoveRule(TwitchTimerRule rule);
    void ClearRules();
}

