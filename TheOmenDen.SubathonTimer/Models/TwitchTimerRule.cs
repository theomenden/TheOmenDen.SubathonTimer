namespace TheOmenDen.SubathonTimer.Models;

public sealed class TwitchTimerRule
{
    public required string EventType { get; set; } // From TwitchEventTypes
    public string? ConditionKey { get; set; }      // Optional match, e.g. "tier"
    public string? ConditionValue { get; set; }    // e.g. "2"
    public TimeSpan TimeToAdd { get; set; }
}
