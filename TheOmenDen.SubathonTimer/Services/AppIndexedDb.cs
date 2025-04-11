using BlazorDexie.Database;
using BlazorDexie.Options;
using TheOmenDen.SubathonTimer.Models.Events;

namespace TheOmenDen.SubathonTimer.Services;

public sealed class AppIndexedDb : Db<AppIndexedDb>
{
    public Store<OfflineEvent, int> Events => new($"++{nameof(OfflineEvent.Id)}", nameof(OfflineEvent.Type), nameof(OfflineEvent.Data));
    public AppIndexedDb(BlazorDexieOptions options) : base("OfflineEventsDb", 1, [], options) { }
}