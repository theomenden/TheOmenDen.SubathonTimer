namespace TheOmenDen.SubathonTimer.Models;

public static class TwitchConstants
{
    public const string TwitchBackend = "CorvidApi";
    public const string TwitchOAuthBaseUri = "https://id.twitch.tv/oauth2/";
    public const string TwitchOAuthHttpClientName = "TwitchOAuth";

    public static class Endpoints
    {
        public const string OAuthToken = "token";
        public const string OAuthAuthorize = "authorize";
        public const string OAuthRevoke = "revoke";
        public const string OAuthValidate = "validate";

    }

    public static class Scopes
    {
        public const string ReadEmail = "user:read:email";
        public const string ReadChat = "user:read:chat";
        public const string ReadFollows = "user:read:follows";
        public const string ReadCharity = "channel:read:charity";
        public const string ReadHypeTrain = "channel:read:hype_train";
        public const string ReadSubscriptions = "channel:read:subscriptions";
        public const string ReadGoals = "channel:read:goals";
        public const string ReadBits = "bits:read";

        public static readonly string[] AllScopes =
        [
            ReadEmail,
            ReadCharity,
            ReadHypeTrain,
            ReadSubscriptions,
            ReadGoals,
            ReadBits
        ];
    }
}