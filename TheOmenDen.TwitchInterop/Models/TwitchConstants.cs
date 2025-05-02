namespace TheOmenDen.TwitchInterop.Models;

public static class TwitchConstants
{
    public const string TwitchHttpClientName = "Twitch";
    public const string TwitchOAuthHttpClientName = "TwitchOAuth";
    public const string TwitchBaseUri = "https://api.twitch.tv/helix/";
    public const string TwitchOAuthBaseUri = "https://id.twitch.tv/oauth2/";
    public const string ClientIdHeader = "Client-Id";
    public const string AuthHeaderScheme = "Bearer";

    public static class Endpoints
    {
        public const string Users = "users";
        public const string EventSubSubscriptions = "eventsub/subscriptions";
        public const string OAuthToken = "token";
        public const string OAuthAuthorize = "authorize";
        public const string OAuthRevoke = "revoke";
        public const string OAuthValidate = "validate";

    }

    public static class EventSubTypes
    {
        public const string StreamOnline = "stream.online";
        public const string StreamOffline = "stream.offline";
        public const string ChannelFollow = "channel.follow";
        public const string ChannelSubscribe = "channel.subscribe";
        public const string ChannelGiftSubscription = "channel.subscribe.gift";
        public const string HypeTrainBegin = "channel.hype_train.begin";
        public const string HypeTrainProgress = "channel.hype_train.progress";
        public const string HypeTrainEnd = "channel.hype_train.end";
        public const string ChannelCheer = "channel.cheer";
    }

    public static class EventSubVersions
    {
        public const string V1 = "1";
        public const string V2 = "2";
    }

    public static class Scopes
    {
        public const string ReadEmail = "user:read:email";
        public const string ReadCharity = "channel:read:charity";
        public const string ReadHypeTrain = "channel:read:hype_train";
        public const string ReadSubscriptions = "channel:read:redemptions";
        public const string ReadGoals = "channel:read:goals";
        public const string ReadBits = "bits:read";
        public const string ManageSubscriptions = "channel:manage:redemptions";
    }

    public static class Headers
    {
        public const string Authorization = "Authorization";
        public const string TwitchMessageId = "Twitch-Eventsub-Message-Id";
        public const string TwitchMessageTimestamp = "Twitch-Eventsub-Message-Timestamp";
        public const string TwitchMessageSignature = "Twitch-Eventsub-Message-Signature";
        public const string TwitchMessageType = "Twitch-Eventsub-Message-Type";
    }
}