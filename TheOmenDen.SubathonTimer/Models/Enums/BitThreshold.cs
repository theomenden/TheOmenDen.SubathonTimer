using Ardalis.SmartEnum;

namespace TheOmenDen.SubathonTimer.Models.Enums;

public abstract class BitThreshold(string name, int value) : SmartEnum<BitThreshold, int>(name, value)
{
    private const string NumericFormat = @"{0} ({1}) {2}";
    public static readonly BitThreshold OneBit = new One();
    public static readonly BitThreshold FiveBits = new Five();
    public static readonly BitThreshold TenBits = new Ten();
    public static readonly BitThreshold FiftyBits = new Fifty();
    public static readonly BitThreshold OneHundredBits = new OneHundred();
    public static readonly BitThreshold TwoHundredFiftyBits = new TwoFifty();
    public static readonly BitThreshold FiveHundredBits = new FiveHundred();
    public static readonly BitThreshold OneThousandBits = new OneThousand();
    public static readonly BitThreshold TwoThousandBits = new TwoThousand();
    public static readonly BitThreshold FiveThousandBits = new FiveThousand();
    public static readonly BitThreshold TenThousandBits = new TenThousand();
    public static readonly BitThreshold[] All = [OneBit, FiveBits, TenBits, FiftyBits, OneHundredBits, TwoHundredFiftyBits, FiveHundredBits, OneThousandBits, TwoThousandBits, FiveThousandBits, TenThousandBits];

    public static BitThreshold FromValue(int value)
    {
        return All.FirstOrDefault(x => x.Value == value) ?? throw new ArgumentOutOfRangeException(nameof(value), $"Unknown bit threshold: {value}");
    }

    public static BitThreshold FromName(string name)
    {
        return All.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? throw new ArgumentOutOfRangeException(nameof(name), $"Unknown bit threshold: {name}");
    }

    public static BitThreshold FromNameOrDefault(string name, BitThreshold defaultValue)
    {
        return All.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? defaultValue;
    }

    public static BitThreshold FromValueOrDefault(int value, BitThreshold defaultValue)
    {
        return All.FirstOrDefault(x => x.Value == value) ?? defaultValue;
    }

    private sealed class One() : BitThreshold(String.Format(NumericFormat, 1, "One", "bit"), 1);
    private sealed class Five() : BitThreshold(String.Format(NumericFormat, 5, "Five", "bits"), 5);
    private sealed class Ten() : BitThreshold(String.Format(NumericFormat, 10, "Ten", "bits"), 10);
    private sealed class Fifty() : BitThreshold(String.Format(NumericFormat, 50, "Fifty", "bits"), 50);
    private sealed class OneHundred() : BitThreshold(String.Format(NumericFormat, 100, "One Hundred", "bits"), 100);
    private sealed class TwoFifty() : BitThreshold(String.Format(NumericFormat, 200, "Two Hundred Fifty", "bits"), 250);
    private sealed class FiveHundred() : BitThreshold(String.Format(NumericFormat, 500, "Five Hundred", "bits"), 500);
    private sealed class OneThousand() : BitThreshold(String.Format(NumericFormat, 1000, "One Thousand", "bits"), 1000);
    private sealed class TwoThousand() : BitThreshold(String.Format(NumericFormat, 2000, "Two Thousand", "bits"), 2000);
    private sealed class FiveThousand() : BitThreshold(String.Format(NumericFormat, 5000, "Five Thousand", "bits"), 5000);
    private sealed class TenThousand() : BitThreshold(String.Format(NumericFormat, 10000, "Ten Thousand", "bits"), 10000);
}