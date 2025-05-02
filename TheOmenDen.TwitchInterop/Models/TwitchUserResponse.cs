using System.Text.Json.Serialization;

namespace TheOmenDen.TwitchInterop.Models;

public sealed record TwitchUserResponse(
    [property: JsonPropertyName("data")] TwitchUser[] Data
);