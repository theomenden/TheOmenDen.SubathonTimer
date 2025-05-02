namespace TheOmenDen.SubathonTimer.Models;

public sealed record OverlayExportSettings(
    bool HasTransparentBackground = true,
    bool EnableAnimation = true,
    string Resolution = "1920x1080"
    );