using Microsoft.JSInterop;
using TheOmenDen.SubathonTimer.Models;

namespace TheOmenDen.SubathonTimer.Services;

internal sealed class BrowserCryptoService(IJSRuntime jsRuntime, AppIndexedDb db) : IUserCryptoService
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private readonly AppIndexedDb _db = db;
    private byte[]? _sessionKey = Array.Empty<byte>();

    private const string KeyId = "overlay-key";

    public async Task<byte[]> GenerateOrLoadKeyAsync(CancellationToken cancellationToken = default)
    {
        if (_sessionKey?.Length > 0) return await Task.FromResult(_sessionKey);

        var existing = await _db.Keys.Get(KeyId, cancellationToken);

        if (existing is not null)
        {
            _sessionKey = existing.RawKey;
            return _sessionKey;
        }

        _sessionKey = await _jsRuntime.InvokeAsync<byte[]>("cryptoService.generateKey", cancellationToken: cancellationToken);
        await _db.Keys.Put(new KeyRecord { Id = KeyId, RawKey = _sessionKey }, cancellationToken);
        return _sessionKey;
    }

    public async Task<byte[]> EncryptAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var key = await GenerateOrLoadKeyAsync(cancellationToken);
        return await _jsRuntime.InvokeAsync<byte[]>("cryptoService.encrypt", cancellationToken: cancellationToken, key, data);
    }

    public async Task<byte[]> DecryptAsync(byte[] encryptedData, CancellationToken cancellationToken = default)
    {
        var key = await GenerateOrLoadKeyAsync(cancellationToken);
        return await _jsRuntime.InvokeAsync<byte[]>("cryptoService.decrypt", cancellationToken: cancellationToken, key, encryptedData);
    }
}