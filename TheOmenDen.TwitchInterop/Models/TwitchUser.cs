using System.Text.Json.Serialization;

namespace TheOmenDen.TwitchInterop.Models;
public sealed record TwitchUser(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("display_name")] string DisplayName,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("profile_image_url")] string? ProfileImageUrl
);