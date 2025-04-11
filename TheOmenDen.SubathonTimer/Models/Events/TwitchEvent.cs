using TheOmenDen.SubathonTimer.Models.Enums;

namespace TheOmenDen.SubathonTimer.Models.Events;

public sealed record TwitchEvent(
    string Id,
    DateTime Timestamp,
    TwitchEventType Type,
    int Bits = 0,
    string Tier = ""
) : IEvent;
