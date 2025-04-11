using TheOmenDen.SubathonTimer.Models.Enums;

namespace TheOmenDen.SubathonTimer.Models;

public record TimerConfiguration
{
    private const string DefaultDisplayFormat = @"hh\:mm\:ss";
    public int InitialTimeSeconds { get; init; } = 300;
    public string DisplayFormat { get; init; } = DefaultDisplayFormat;
    public FluentTheme Theme { get; init; } = FluentTheme.Light;
    public string? CustomColorHex { get; init; }
    public string FontSize { get; init; } = "5rem";
    public string Color { get; init; } = "white";

    public Dictionary<TwitchSubscriptionTier, int> SubBoosts { get; init; } = new()
    {
        [TwitchSubscriptionTier.Prime] = 45,
        [TwitchSubscriptionTier.Tier1] = 30,
        [TwitchSubscriptionTier.Tier2] = 60,
        [TwitchSubscriptionTier.Tier3] = 90
    };

    public Dictionary<BitThreshold, int> BitsBoostTable { get; init; } = new()
    {
        [BitThreshold.One] = 1,
        [BitThreshold.Five] = 2,
        [BitThreshold.Ten] = 5,
        [BitThreshold.OneHundred] = 10,
        [BitThreshold.OneThousand] = 60
    };
}
