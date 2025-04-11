using TheOmenDen.SubathonTimer.Models.Events;

namespace TheOmenDen.SubathonTimer.Services.EventBus;

public interface IEventHandler<in T> where T : IEvent
{
    ValueTask HandleAsync(T @event, CancellationToken token);
}