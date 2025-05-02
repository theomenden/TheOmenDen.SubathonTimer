using System.Text.Json;
using TheOmenDen.SubathonTimer.Models;

namespace TheOmenDen.SubathonTimer.Services;

public sealed class TimerService : ITimerService
{
    private readonly List<TwitchTimerRule> _rules = [];
    private TimeSpan _time = TimeSpan.Zero;
    private TimeSpan? _defaultTime;

    private System.Timers.Timer? _countdownTimer;

    public TimeSpan RemainingTime => _time;
    public TimeSpan? DefaultTime => _defaultTime;

    public event Action? OnTimerUpdated;

    public TimerService()
    {
        _countdownTimer = new System.Timers.Timer(1000); // 1 second
        _countdownTimer.Elapsed += (s, e) =>
        {
            if (_time > TimeSpan.Zero)
            {
                _time -= TimeSpan.FromSeconds(1);
                OnTimerUpdated?.Invoke();
            }
        };
        _countdownTimer.Start();
    }

    public void SetTime(TimeSpan time)
    {
        _time = time;
        _defaultTime = time;
        OnTimerUpdated?.Invoke();
    }

    public void Reset()
    {
        if (_defaultTime.HasValue)
        {
            _time = _defaultTime.Value;
            OnTimerUpdated?.Invoke();
        }
    }

    public void ZeroOut()
    {
        _time = TimeSpan.Zero;
        OnTimerUpdated?.Invoke();
    }

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

            if (!string.IsNullOrWhiteSpace(rule.ConditionKey) &&
                eventData.TryGetProperty(rule.ConditionKey, out var val))
            {
                if (!string.Equals(val.ToString(), rule.ConditionValue, StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            _time += rule.TimeToAdd;
            OnTimerUpdated?.Invoke();
        }
    }

    public IReadOnlyCollection<TwitchTimerRule> GetRules() => _rules.AsReadOnly();

    public void AddRule(TwitchTimerRule rule) => _rules.Add(rule);
    public void RemoveRule(TwitchTimerRule rule) => _rules.Remove(rule);
    public void ClearRules() => _rules.Clear();
}
