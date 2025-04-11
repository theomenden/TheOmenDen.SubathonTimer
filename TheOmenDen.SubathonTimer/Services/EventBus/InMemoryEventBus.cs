using System.Collections.Concurrent;
using TheOmenDen.SubathonTimer.Models.Events;

namespace TheOmenDen.SubathonTimer.Services.EventBus;

public sealed class InMemoryEventBus : IEventBus
{
    private readonly ILogger<InMemoryEventBus> _logger;
    private readonly ConcurrentDictionary<Type, IEventHandlerRegistration> _registrations = new();

    public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
    {
        _logger = logger;
    }

    public void RegisterHandler<T>(IEventHandler<T> handler) where T : IEvent
    {
        var registration = (EventHandlerRegistration<T>)_registrations.GetOrAdd(
            typeof(T),
            _ => new EventHandlerRegistration<T>());

        registration.AddHandler(handler);
        _logger.LogInformation("Registered handler for event {EventType}", typeof(T).Name);
    }

    public async ValueTask PublishAsync<T>(T @event, CancellationToken token = default) where T : IEvent
    {
        if (_registrations.TryGetValue(typeof(T), out var reg) &&
            reg is EventHandlerRegistration<T> typedReg)
        {
            await typedReg.DispatchAsync(@event, token);
        }
        else
        {
            _logger.LogWarning("No handler registered for event type {EventType}", typeof(T).Name);
        }
    }
}
