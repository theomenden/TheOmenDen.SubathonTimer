namespace TheOmenDen.SubathonTimer.Models;

public sealed class TwitchUserState
{
    public TwitchUserInfo? User { get; set; }

    public bool IsLoggedIn => User is not null;

    public void Clear()
    {
        User = null;
    }
}
