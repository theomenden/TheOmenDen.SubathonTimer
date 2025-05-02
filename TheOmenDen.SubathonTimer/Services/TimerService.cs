using System.Text.Json;
using TheOmenDen.SubathonTimer.Models;

namespace TheOmenDen.SubathonTimer.Services;

public sealed class TimerService : ITimerService
{
    private readonly List<TwitchTimerRule> _rules = [];
    private TimeSpan _time = TimeSpan.Zero;

    public TimeSpan RemainingTime => _time;

    public event Action? OnTimerUpdated;

    public void AddManual(TimeSpan time)
    {
        _time += time;
        OnTimerUpdated?.Invoke();
    }

    public void SubtractManual(TimeSpan time)
    {
        _time = _time > time ? _time - time : TimeSpan.Zero;
        OnTimerUpdated?.Invoke();
    }

    public void HandleTwitchEvent(string eventType, JsonElement eventData)
    {
        foreach (var rule in _rules)
        {
            if (!string.Equals(rule.EventType, eventType, StringComparison.OrdinalIgnoreCase))
                continue;

            if (!string.IsNullOrWhiteSpace(rule.ConditionKey) && eventData.TryGetProperty(rule.ConditionKey, out var val))
            {
                if (!string.Equals(val.ToString(), rule.ConditionValue, StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            _time += rule.TimeToAdd;
            OnTimerUpdated?.Invoke();
        }
    }

    public IReadOnlyCollection<TwitchTimerRule> GetRules() => _rules.AsReadOnly();

    public void AddRule(TwitchTimerRule rule)
    {
        _rules.Add(rule);
    }

    public void RemoveRule(TwitchTimerRule rule)
    {
        _rules.Remove(rule);
    }

    public void ClearRules() => _rules.Clear();
}
