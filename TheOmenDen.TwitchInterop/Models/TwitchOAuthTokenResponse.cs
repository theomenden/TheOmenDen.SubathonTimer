using System.Text.Json.Serialization;

namespace TheOmenDen.TwitchInterop.Models;

public sealed record TwitchOAuthTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("refresh_token")] string RefreshToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("scope")] string[] Scope,
    [property: JsonPropertyName("token_type")] string TokenType
);
