using Microsoft.AspNetCore.Components;
using TheOmenDen.SubathonTimer.Services;

namespace TheOmenDen.SubathonTimer.Pages;

public partial class Overlay : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; init; } = default!;
    [Inject] private IOverlayConfigService ConfigService { get; init; } = default!;
    [Inject] private IOverlaySignatureHelper OverlaySignatureHelper { get; init; } = default!;

    private bool _isVerified = false;

    protected override async Task OnInitializedAsync()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var channel = query["channel"] ?? String.Empty;
        var ts = query["ts"] ?? String.Empty;
        var signature = query["sig"] ?? String.Empty;

        _isVerified = await OverlaySignatureHelper.VerifySignatureAsync(channel, ts, signature);

        if (_isVerified)
        {
            await ConfigService.LoadConfigAsync(channel);
            return;
        }

        // If not verified, redirect to an error page or show an error message
        NavigationManager.NavigateTo("/error");
    }
}