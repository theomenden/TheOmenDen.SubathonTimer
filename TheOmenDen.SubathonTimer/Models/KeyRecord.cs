namespace TheOmenDen.SubathonTimer.Models;

public sealed class KeyRecord
{
    public string Id { get; set; } = "overlay-key";
    public byte[] RawKey { get; set; } = Array.Empty<byte>();
}