using System.Text.Json.Serialization;

namespace TheOmenDen.TwitchInterop.Models;

public sealed record TwitchEventSub(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("version")] string Version
);