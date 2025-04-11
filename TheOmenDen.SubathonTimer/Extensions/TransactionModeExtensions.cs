using System.Collections.Frozen;
using TheOmenDen.SubathonTimer.Models.Enums;

namespace TheOmenDen.SubathonTimer.Extensions;

public static class TransactionModeExtensions
{
    private static FrozenDictionary<TransactionMode, string> ModeToNames =>
        new Dictionary<TransactionMode, string>()
        {
            { TransactionMode.Read, "r" },
            { TransactionMode.Write, "w" },
            { TransactionMode.ReadWrite, "rw" },
            { TransactionMode.ReadQ, "r?" },
            { TransactionMode.WriteQ, "w?" },
            { TransactionMode.ReadWriteQ, "rw?" },
            { TransactionMode.ReadEx, "r!" },
            { TransactionMode.WriteEx, "w!" },
            { TransactionMode.ReadWriteEx, "rw!" }
           }.ToFrozenDictionary();

    private static FrozenDictionary<string, TransactionMode> NamesToMode =>
        new Dictionary<string, TransactionMode>(StringComparer.OrdinalIgnoreCase)
        {
            { "r", TransactionMode.Read },
            { "w", TransactionMode.Write },
            { "rw", TransactionMode.ReadWrite },
            { "r?", TransactionMode.ReadQ },
            { "w?", TransactionMode.WriteQ },
            { "rw?", TransactionMode.ReadWriteQ },
            { "r!", TransactionMode.ReadEx },
            { "w!", TransactionMode.WriteEx },
            { "rw!", TransactionMode.ReadWriteEx }
        }.ToFrozenDictionary();

    public static string ToName(this TransactionMode mode) => (ModeToNames.TryGetValue(mode, out var name) ? name : null) ?? string.Empty;

    public static bool TryFromName(string name, out TransactionMode mode) => NamesToMode.TryGetValue(name, out mode);

    public static TransactionMode FromName(string name)
    {
        if (NamesToMode.TryGetValue(name, out var mode))
        {
            return mode;
        }
        throw new ArgumentException($"Invalid transaction mode name: {name}", nameof(name));
    }
}