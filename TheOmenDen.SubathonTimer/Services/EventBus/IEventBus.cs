using TheOmenDen.SubathonTimer.Models.Events;

namespace TheOmenDen.SubathonTimer.Services.EventBus;

public interface IEventBus
{
    ValueTask PublishAsync<T>(T @event, CancellationToken token = default) where T : IEvent;
    void RegisterHandler<T>(IEventHandler<T> handler) where T : IEvent;
}