using System.Collections.Frozen;
using TheOmenDen.SubathonTimer.Models.Enums;

namespace TheOmenDen.SubathonTimer.Extensions;

public static class TwitchSubscriptionTierExtensions
{
    private static readonly FrozenDictionary<string, TwitchSubscriptionTier> StringToTier =
        new Dictionary<string, TwitchSubscriptionTier>(StringComparer.OrdinalIgnoreCase)
        {
            ["prime"] = TwitchSubscriptionTier.Prime,
            ["1000"] = TwitchSubscriptionTier.Tier1,
            ["2000"] = TwitchSubscriptionTier.Tier2,
            ["3000"] = TwitchSubscriptionTier.Tier3
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<TwitchSubscriptionTier, string> TierToString =
        new Dictionary<TwitchSubscriptionTier, string>
        {
            [TwitchSubscriptionTier.Prime] = "prime",
            [TwitchSubscriptionTier.Tier1] = "1000",
            [TwitchSubscriptionTier.Tier2] = "2000",
            [TwitchSubscriptionTier.Tier3] = "3000"
        }.ToFrozenDictionary();

    /// <summary>
    /// Parses the specified raw tier string into its corresponding enum value.
    /// </summary>
    /// <param name="raw">The raw.</param>
    /// <returns>The parsed <see cref="TwitchSubscriptionTier" /> value.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">raw - Unknown tier string: {raw}</exception>
    public static TwitchSubscriptionTier Parse(string raw)
    {
        if (StringToTier.TryGetValue(raw, out var tier))
            return tier;

        throw new ArgumentOutOfRangeException(nameof(raw), $"Unknown tier string: {raw}");
    }

    /// <summary>
    /// Converts to raw string representation of the tier.
    /// </summary>
    /// <param name="tier">The tier.</param>
    /// <returns>The raw string representation of the tier. If the tier is not recognized, returns "1000".</returns>
    public static string ToRaw(this TwitchSubscriptionTier tier)
    {
        return TierToString.TryGetValue(tier, out var str) ? str : "1000";
    }
}