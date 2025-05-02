namespace TheOmenDen.SubathonTimer.Services;

using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using TheOmenDen.SubathonTimer.Models;

public interface ITimerSignalRService
{
    Task ConnectAsync();
}

public sealed class TimerSignalRService(IHttpClientFactory factory, ITimerService timer)
{
    private readonly ITimerService _timer = timer;
    private HubConnection? _connection;

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        var httpClient = factory.CreateClient(TwitchConstants.TwitchBackend);
        using var response = await httpClient.PostAsync("negotiate", null, cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to negotiate SignalR connection.");
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var connectionInfo = JsonSerializer.Deserialize<SignalRConnectionInfoResponse>(json)!;

        _connection = new HubConnectionBuilder()
            .WithUrl(connectionInfo.Url, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(connectionInfo.AccessToken);
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.On<string, string>("twitchEvent", (eventType, jsonData) =>
        {
            using var doc = JsonDocument.Parse(jsonData);
            _timer.HandleTwitchEvent(eventType, doc.RootElement);
        });

        await _connection.StartAsync(cancellationToken);
    }
}
