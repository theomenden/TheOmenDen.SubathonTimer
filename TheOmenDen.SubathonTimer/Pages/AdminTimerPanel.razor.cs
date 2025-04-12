using Microsoft.AspNetCore.Components;

namespace TheOmenDen.SubathonTimer.Pages;

public partial class AdminTimerPanel : ComponentBase
{
    [Parameter]
    public EventCallback<int> OnExtend { get; set; }

    [Parameter]
    public EventCallback OnPause { get; set; }

    [Parameter]
    public EventCallback OnResume { get; set; }

    [Parameter]
    public EventCallback OnReset { get; set; }

    [Parameter]
    public TimeSpan Remaining { get; set; }

    [Parameter]
    public string Format { get; set; } = @"hh\:mm\:ss";

    [Inject] private ILogger<AdminTimerPanel> Logger { get; init; } = default!;

    private TimeSpan _remaining => Remaining;
    private bool _isPaused;
    private string _customExtendStr = "15";

    private async Task PauseOrResume()
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            Logger.LogInformation("Timer paused.");
            await OnPause.InvokeAsync();
        }
        else
        {
            Logger.LogInformation("Timer resumed.");
            await OnResume.InvokeAsync();
        }
    }


    private async Task ResetTimer() => await OnReset.InvokeAsync();

    private async Task ExtendTime(int seconds)
    {
        Logger.LogInformation("Timer extended by {Seconds}s from admin.", seconds);
        await OnExtend.InvokeAsync(seconds);
    }

    private async Task ApplyCustomExtend()
    {
        if (int.TryParse(_customExtendStr, out var seconds) && seconds > 0)
        {
            await ExtendTime(seconds);
        }
    }
}