using System.Text.Json.Serialization;

namespace TheOmenDen.SubathonTimer.Models.Events;

public sealed class OfflineEvent
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Id { get; set; }
    public string Type { get; set; } = String.Empty;
    public string Data { get; set; } = String.Empty;
}