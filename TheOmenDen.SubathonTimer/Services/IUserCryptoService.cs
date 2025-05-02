namespace TheOmenDen.SubathonTimer.Services;

public interface IUserCryptoService
{
    Task<byte[]> GenerateOrLoadKeyAsync(CancellationToken cancellationToken = default);
    Task<byte[]> EncryptAsync(byte[] data, CancellationToken cancellationToken = default);
    Task<byte[]> DecryptAsync(byte[] encryptedData, CancellationToken cancellationToken = default);
}