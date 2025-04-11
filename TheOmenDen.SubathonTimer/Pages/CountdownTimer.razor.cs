using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TheOmenDen.SubathonTimer.Services;
using TheOmenDen.SubathonTimer.Services.EventBus;

namespace TheOmenDen.SubathonTimer.Pages;

public partial class CountdownTimer : ComponentBase
{
    [Parameter] public EventCallback<TimeSpan> OnTick { get; set; }
    [Parameter] public string Format { get; set; } = @"hh\:mm\:ss";
    [Inject] private IEventBus EventBus { get; init; } = default!;
    [Inject] TimerConfigService ConfigService { get; init; } = default!;
    [Inject] ILogger<CountdownTimer> Logger { get; init; } = default!;
    [Inject] IJSRuntime JS { get; init; } = default!;

    private TimeSpan _remaining;
    private System.Timers.Timer? _timer;
    private string _format = @"hh\:mm\:ss";
    private bool _isRunning;
    private string FontSize => ConfigService.Config.FontSize;
    private string Color => ConfigService.Config.Color;
    private string _styleToken = String.Empty;

    protected override async Task OnInitializedAsync()
    {
        // Load runtime configuration
        await ConfigService.LoadAsync();
        _remaining = TimeSpan.FromSeconds(ConfigService.Config.InitialTimeSeconds);
        _format = ConfigService.Config.DisplayFormat;
        _styleToken = $"font-size: {FontSize}; color: {Color};";
        // Register this timer's event handler
        EventBus.RegisterHandler(new TimerEventHandler(this, Logger, ConfigService, JS));

        // Start the countdown
        StartTimer();
    }

    public TimeSpan Remaining => _remaining;

    public void Pause()
    {
        _timer?.Stop();
        _isRunning = false;
    }

    public void Resume()
    {
        if (!_isRunning)
            StartTimer();
    }

    private void StartTimer()
    {
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (s, e) =>
        {
            if (_remaining.TotalSeconds > 0)
            {
                _remaining = _remaining.Subtract(TimeSpan.FromSeconds(1));
                OnTick.InvokeAsync(_remaining);
                InvokeAsync(StateHasChanged);
            }
            else
            {
                _timer?.Stop();
                _isRunning = false;
                Logger.LogInformation("Countdown finished.");
            }
        };

        _timer.Start();
        _isRunning = true;
    }

    public void ExtendTime(int seconds)
    {
        if (seconds <= 0)
            return;

        _remaining = _remaining.Add(TimeSpan.FromSeconds(seconds));
        _ = JS.InvokeVoidAsync("shakeElement", "timer-display");
        InvokeAsync(StateHasChanged);

        Logger.LogInformation("Timer extended by {Seconds}s. New time: {Time}", seconds, _remaining);
    }

    public void Reset() => _remaining = TimeSpan.FromSeconds(ConfigService.Config.InitialTimeSeconds);
}