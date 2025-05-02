using System.Collections.Concurrent;
using TheOmenDen.SubathonTimer.Models;

namespace TheOmenDen.SubathonTimer.Services;

public interface IOverlayConfigService
{
    Task<OverlayExportSettings> LoadConfigAsync(string channel, CancellationToken cancellationToken = default);
    Task SaveConfigAsync(string channel, OverlayExportSettings settings, CancellationToken cancellationToken = default);
}

internal sealed class OverlayConfigService : IOverlayConfigService
{
    private readonly ConcurrentDictionary<string, OverlayExportSettings> _configsConcurrentDictionary = [];

    public Task<OverlayExportSettings> LoadConfigAsync(string channel, CancellationToken cancellationToken = default)
    {
        if (_configsConcurrentDictionary.TryGetValue(channel, out var settings))
        {
            return Task.FromResult(settings);
        }
        // If not found, return default settings
        var defaultSettings = new OverlayExportSettings();
        _configsConcurrentDictionary[channel] = defaultSettings;
        return Task.FromResult(defaultSettings);
    }

    public Task SaveConfigAsync(string channel, OverlayExportSettings settings, CancellationToken cancellationToken = default)
    {
        _configsConcurrentDictionary[channel] = settings;
        return Task.CompletedTask;
    }
}