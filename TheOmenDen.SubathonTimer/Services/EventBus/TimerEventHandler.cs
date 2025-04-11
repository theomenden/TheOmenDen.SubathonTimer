using Microsoft.JSInterop;
using TheOmenDen.SubathonTimer.Extensions;
using TheOmenDen.SubathonTimer.Models.Enums;
using TheOmenDen.SubathonTimer.Models.Events;
using TheOmenDen.SubathonTimer.Pages;

namespace TheOmenDen.SubathonTimer.Services.EventBus;


public class TimerEventHandler(
    CountdownTimer timerComponent,
    ILogger<CountdownTimer> logger,
    TimerConfigService configService,
    IJSRuntime js)
    : IEventHandler<TwitchEvent>
{
    public ValueTask HandleAsync(TwitchEvent evt, CancellationToken token)
    {
        var seconds = evt.Type switch
        {
            TwitchEventType.Cheer => MapBitsToTime(evt.Bits),
            TwitchEventType.Sub or TwitchEventType.Resub => MapSubToTime(evt.Tier),
            _ => 0
        };

        if (seconds > 0)
        {
            logger.LogInformation("Timer extended by {Seconds} due to {Type} event", seconds, evt.Type);
            timerComponent.ExtendTime(seconds);
        }

        return ValueTask.CompletedTask;
    }

    private int MapBitsToTime(int bits)
    {
        var config = configService.Config.BitsBoostTable;
        return config
            .OrderByDescending(b => (int)b.Key)
            .FirstOrDefault(entry => bits >= (int)entry.Key).Value;
    }

    private int MapSubToTime(string rawTier)
    {
        var tier = TwitchSubscriptionTierExtensions.Parse(rawTier);
        return configService.Config.SubBoosts.TryGetValue(tier, out var boost) ? boost : 0;
    }
}