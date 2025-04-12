using System.Security.Cryptography;
using System.Text;

namespace TheOmenDen.SubathonTimer.Services;

public static class OverlaySignatureHelper
{
    private const string SecretKey = "YOUR_SUPER_SECRET_KEY"; // replace w/ key vault lookup in prod

    public static string Generate(string channel)
    {
        var key = Encoding.UTF8.GetBytes(SecretKey);
        var msg = Encoding.UTF8.GetBytes(channel.ToLowerInvariant());

        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(msg);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static bool IsValid(string channel, string sig)
    {
        var expected = Generate(channel);
        return string.Equals(expected, sig, StringComparison.OrdinalIgnoreCase);
    }
}