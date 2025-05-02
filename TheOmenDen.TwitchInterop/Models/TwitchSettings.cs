namespace TheOmenDen.TwitchInterop.Models;

public sealed class TwitchSettings
{
    public required string ClientId { get; set; } = String.Empty;
    public required string ClientSecret { get; set; } = String.Empty;

    public required string SigningSecret { get; set; } = String.Empty;
}