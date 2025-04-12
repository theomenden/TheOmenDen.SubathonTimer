using Microsoft.AspNetCore.Components;

namespace TheOmenDen.SubathonTimer.Pages;

public partial class AdminDashboard : ComponentBase
{
    [Inject] private ILogger<AdminDashboard> Logger { get; set; } = default!;
    private CountdownTimer? _timerComponent;
    private TimeSpan _remaining;
    private string _format = @"hh\:mm\:ss";

    private Task UpdateRemaining(TimeSpan newTime)
    {
        _remaining = newTime;
        StateHasChanged(); // ensure the admin panel updates
        return Task.CompletedTask;
    }

    private Task ExtendTime(int seconds)
    {
        Logger.LogInformation("Admin requested +{Seconds}s", seconds);
        _timerComponent?.ExtendTime(seconds);
        return Task.CompletedTask;
    }

    private Task Pause()
    {
        Logger.LogInformation("Admin paused timer");
        _timerComponent?.Pause();
        return Task.CompletedTask;
    }

    private Task Resume()
    {
        Logger.LogInformation("Admin resumed timer");
        _timerComponent?.Resume();
        return Task.CompletedTask;
    }

    private Task Reset()
    {
        Logger.LogInformation("Admin reset timer");
        _timerComponent?.Reset();
        return Task.CompletedTask;
    }
}