using System.Text.Json.Serialization;

namespace TheOmenDen.TwitchInterop.Models;

public sealed record TwitchEventSubResponse(
    [property: JsonPropertyName("data")] TwitchEventSub[] Data
);