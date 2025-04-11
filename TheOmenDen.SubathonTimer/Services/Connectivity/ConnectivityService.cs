namespace TheOmenDen.SubathonTimer.Services.Connectivity;

public sealed class ConnectivityService : IConnectivityService
{
    public bool IsOnline { get; private set; } = true;
    public event Action<bool>? OnStatusChanged;

    public void SetOnlineStatus(bool isOnline)
    {
        if (IsOnline != isOnline)
        {
            IsOnline = isOnline;
            OnStatusChanged?.Invoke(isOnline);
        }
    }
}