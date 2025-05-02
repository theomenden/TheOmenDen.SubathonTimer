using System.Text.Json.Serialization;

namespace TheOmenDen.TwitchInterop.Models;

public sealed record TwitchEventSubSubscriptionRequest(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("user_id")] string UserId
);
