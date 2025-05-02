using System.Text.Json.Serialization;

namespace TheOmenDen.TwitchInterop.Models;
sealed record AccessTokenRequest(
    [property: JsonPropertyName("access_token")] string AccessToken
);