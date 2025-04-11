using TheOmenDen.SubathonTimer.Models;

namespace TheOmenDen.SubathonTimer.Services;

public sealed class TimerConfigService
{
    public TimerConfiguration Config { get; private set; } = new(); // record (immutable by default)

    public event Func<TimerConfiguration, CancellationToken, Task>? OnConfigChanged;

    public Task LoadAsync()
    {
        Config = new TimerConfiguration(); // In a real app, load from localStorage/IndexedDB/API
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(TimerConfiguration updated, CancellationToken cancellationToken = default)
    {
        Config = updated;
        await OnConfigChanged?.Invoke(Config, cancellationToken);
    }
}

