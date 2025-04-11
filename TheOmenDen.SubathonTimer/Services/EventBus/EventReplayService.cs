using TheOmenDen.SubathonTimer.Models.Events;
using TheOmenDen.SubathonTimer.Services.Connectivity;

namespace TheOmenDen.SubathonTimer.Services.EventBus;

public sealed class EventReplayService
{
    private readonly IConnectivityService _connectivity;
    private readonly IEventStore _eventStore;
    private readonly IEventBus _eventBus;
    private readonly ILogger<EventReplayService> _logger;

    public EventReplayService(
        IConnectivityService connectivity,
        IEventStore eventStore,
        IEventBus eventBus,
        ILogger<EventReplayService> logger)
    {
        _connectivity = connectivity;
        _eventStore = eventStore;
        _eventBus = eventBus;
        _logger = logger;

        _connectivity.OnStatusChanged += async isOnline =>
        {
            if (isOnline)
            {
                await ReplayAllEventsAsync();
            }
        };
    }

    private async Task ReplayAllEventsAsync()
    {
        try
        {
            _logger.LogInformation("Replaying queued offline events...");

            var twitchEvents = await _eventStore.DequeueAllAsync<TwitchEvent>();

            foreach (var evt in twitchEvents)
            {
                _logger.LogInformation("Replaying event: {Type} ({Id})", evt.Type, evt.Id);
                await _eventBus.PublishAsync(evt);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to replay offline events.");
        }
    }
}