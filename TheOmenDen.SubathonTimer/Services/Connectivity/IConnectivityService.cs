namespace TheOmenDen.SubathonTimer.Services.Connectivity;

public interface IConnectivityService
{
    bool IsOnline { get; }
    event Action<bool> OnStatusChanged;
    void SetOnlineStatus(bool isOnline);
}