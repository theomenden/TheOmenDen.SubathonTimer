using System.IO.Compression;
using System.Text;
using System.Text.Json;
using TheOmenDen.SubathonTimer.Extensions;
using TheOmenDen.SubathonTimer.Models.Enums;
using TheOmenDen.SubathonTimer.Models.Events;

namespace TheOmenDen.SubathonTimer.Services.EventBus;

public sealed class DexieEventStore(AppIndexedDb db) : IEventStore
{
    public async ValueTask EnqueueAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent
    {
        var json = JsonSerializer.Serialize(@event);
        var compressed = Convert.ToBase64String(Compress(Encoding.UTF8.GetBytes(json)));

        var record = new OfflineEvent
        {
            Type = typeof(T).Name,
            Data = compressed
        };

        await db.Transaction(TransactionMode.ReadWrite.ToName(), [nameof(AppIndexedDb.Events)], 60_000, async () => await db.Events.Add(record, cancellationToken), cancellationToken);
    }

    public async ValueTask<List<T>> DequeueAllAsync<T>(CancellationToken cancellationToken = default) where T : IEvent
    {
        var typeName = typeof(T).Name;
        var list = new List<T>();

        await db.Transaction(TransactionMode.ReadWrite.ToName(), [nameof(AppIndexedDb.Events)], 60_000, async () =>
        {
            var records = await db.Events
                .Where(nameof(OfflineEvent))
                .EqualIgnoreCase(typeName)
                .ToArray(cancellationToken);

            foreach (var r in records)
            {
                var decompressed = Decompress(Convert.FromBase64String(r.Data));
                var parsed = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(decompressed));
                if (parsed is null) continue;
                list.Add(parsed);
                await db.Events.Delete(r.Id, cancellationToken);
            }
        }, cancellationToken);

        return list;
    }

    private static byte[] Compress(byte[] input)
    {
        using var output = new MemoryStream();
        using var gzip = new GZipStream(output, CompressionMode.Compress);
        gzip.Write(input, 0, input.Length);
        return output.ToArray();
    }

    private static byte[] Decompress(byte[] input)
    {
        using var inputStream = new MemoryStream(input);
        using var gzip = new GZipStream(inputStream, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }
}