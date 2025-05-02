using Azure;

namespace TheOmenDen.TwitchInterop.Models;

public sealed class TwitchUserMap
{
    public string PartitionKey { get; set; } = "TwitchUser";
    public string RowKey { get; set; } = default!; // Twitch broadcaster_user_id
    public string AzureUserId { get; set; } = default!; // Entra OID
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}