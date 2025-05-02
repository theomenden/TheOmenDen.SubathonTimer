using Microsoft.AspNetCore.SignalR.Client;
using System.Data.Common;

namespace TheOmenDen.SubathonTimer.Services.Connectivity;

public sealed class SignalRService
{
    private HubConnection? _hubConnection;

    public async Task StartAsync(string Token, CancellationToken cancellationToken = default)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://twitchinterop-g9h9b6fyh8f3f0ga.eastus2-01.azurewebsites.net/api/hub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(Token);
            })
            .Build();
        _hubConnection.On<string, string>("twitchEvent", (eventType, payload) =>
        {
            // TODO: Display or route event
            Console.WriteLine($"Received Twitch event: {eventType} — {payload}");
        });

        await _hubConnection.StartAsync(cancellationToken);

    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return _hubConnection is not null ? _hubConnection.StopAsync(cancellationToken) : Task.CompletedTask;
    }
}