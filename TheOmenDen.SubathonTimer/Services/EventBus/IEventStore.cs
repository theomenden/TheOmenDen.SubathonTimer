using TheOmenDen.SubathonTimer.Models.Events;

namespace TheOmenDen.SubathonTimer.Services.EventBus;

public interface IEventStore
{
    /// <summary>
    /// Store an event (persistently if needed).
    /// </summary>
    ValueTask EnqueueAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent;

    /// <summary>
    /// Load and remove all events of a specific type.
    /// </summary>
    ValueTask<List<T>> DequeueAllAsync<T>(CancellationToken cancellationToken = default) where T : IEvent;
}
