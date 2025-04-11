using TheOmenDen.SubathonTimer.Models.Events;

namespace TheOmenDen.SubathonTimer.Services.EventBus;

public sealed class EventHandlerRegistration<T> : IEventHandlerRegistration where T : IEvent
{
    private readonly List<IEventHandler<T>> _handlers = new();

    public void AddHandler(IEventHandler<T> handler) => _handlers.Add(handler);

    public async Task DispatchAsync(T @event, CancellationToken token)
    {
        foreach (var handler in _handlers)
        {
            await handler.HandleAsync(@event, token);
        }
    }
}
