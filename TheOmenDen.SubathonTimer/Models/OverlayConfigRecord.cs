namespace TheOmenDen.SubathonTimer.Models;

public sealed class OverlayConfigRecord
{
    public string Channel { get; set; } = String.Empty;
    public OverlayExportSettings Settings { get; set; } = new();
}