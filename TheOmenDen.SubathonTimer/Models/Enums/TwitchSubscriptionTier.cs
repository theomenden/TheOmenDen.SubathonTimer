using Ardalis.SmartEnum;

namespace TheOmenDen.SubathonTimer.Models.Enums;

//public enum TwitchSubscriptionTier
//{
//    Prime,
//    Tier1,
//    Tier2,
//    Tier3
//}

public abstract class TwitchSubscriptionTier(string name, int value) : SmartEnum<TwitchSubscriptionTier, int>(name, value)
{
    public static readonly TwitchSubscriptionTier Prime = new PrimeTier();
    public static readonly TwitchSubscriptionTier Tier1 = new SubTier1();
    public static readonly TwitchSubscriptionTier Tier2 = new SubTier2();
    public static readonly TwitchSubscriptionTier Tier3 = new SubTier3();
    public static readonly TwitchSubscriptionTier[] All = { Prime, Tier1, Tier2, Tier3 };


    public static TwitchSubscriptionTier FromValue(int value)
    {
        return All.FirstOrDefault(x => x.Value == value) ?? throw new ArgumentOutOfRangeException(nameof(value), $"Unknown subscription tier: {value}");
    }

    public static TwitchSubscriptionTier FromName(string name)
    {
        return All.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? throw new ArgumentOutOfRangeException(nameof(name), $"Unknown subscription tier: {name}");
    }

    public static TwitchSubscriptionTier FromNameOrDefault(string name, TwitchSubscriptionTier defaultValue)
    {
        return All.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? defaultValue;
    }

    public static TwitchSubscriptionTier FromValueOrDefault(int value, TwitchSubscriptionTier defaultValue)
    {
        return All.FirstOrDefault(x => x.Value == value) ?? defaultValue;
    }

    private sealed class PrimeTier() : TwitchSubscriptionTier("Prime", 0);
    private sealed class SubTier1() : TwitchSubscriptionTier("Tier 1", 1);
    private sealed class SubTier2() : TwitchSubscriptionTier("Tier 2", 2);
    private sealed class SubTier3() : TwitchSubscriptionTier("Tier 3", 3);
}
