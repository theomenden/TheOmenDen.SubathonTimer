using System.Security.Cryptography;
using System.Text;

namespace TheOmenDen.SubathonTimer.Services;

public interface IOverlaySignatureHelper
{
    Task<string> GenerateSignedUrlAsync(string channel, string timestamp, CancellationToken cancellationToken = default);
    Task<bool> VerifySignatureAsync(string channel, string ts, string providedSig, CancellationToken cancellationToken = default);
}

internal sealed class OverlaySignatureHelper(IUserCryptoService cryptoService) : IOverlaySignatureHelper
{
    private readonly IUserCryptoService _cryptoService = cryptoService;

    public async Task<string> GenerateSignedUrlAsync(string channel, string timestamp, CancellationToken cancellationToken = default)
    {
        var key = await _cryptoService.GenerateOrLoadKeyAsync(cancellationToken);
        var raw = Encoding.UTF8.GetBytes($"{channel}{timestamp}");

        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(raw);

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public async Task<bool> VerifySignatureAsync(string channel, string ts, string providedSig, CancellationToken cancellationToken = default)
    {
        var expected = await GenerateSignedUrlAsync(channel, ts, cancellationToken);
        return String.Equals(expected, providedSig, StringComparison.OrdinalIgnoreCase);
    }
}