using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using TheOmenDen.SubathonTimer.Models;
using TheOmenDen.SubathonTimer.Services;

namespace TheOmenDen.SubathonTimer.Pages;

public partial class Overlay : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; init; } = default!;
    [Inject] private ITimerService Timer { get; init; } = default!;
    [Inject] private IHttpClientFactory HttpClientFactory { get; init; } = default!;

    private bool _isVerified = false;
    [Parameter] public string BroadcasterId { get; set; } = string.Empty;

    private TimeSpan _remaining = TimeSpan.Zero;
    private string DisplayTime => _remaining.ToString(@"hh\:mm\:ss");

    private HubConnection? _hubConnection;

    protected override async Task OnInitializedAsync()
    {
        Timer.OnTimerUpdated += StateHasChanged;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{NavigationManager.BaseUri}api/negotiate", options =>
            {
                options.Headers.Add("x-broadcaster-id", "{your-id-here}"); // dynamic ID
                options.Transports = HttpTransportType.WebSockets;
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string, string>("twitchEvent", (eventType, json) =>
        {
            var doc = JsonDocument.Parse(json);
            Timer.HandleTwitchEvent(eventType, doc.RootElement);
        });

        await _hubConnection.StartAsync();
    }

    public async Task ConnectAsync()
    {
        // No need to add headers — Easy Auth handles x-ms-client-principal-id
        var httpClient = HttpClientFactory.CreateClient(TwitchConstants.TwitchBackend);

        var negotiateResponse = await httpClient.PostAsync("api/negotiate", null);
        if (!negotiateResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to negotiate SignalR connection.");
        }

        var json = await negotiateResponse.Content.ReadAsStringAsync();
        var connectionInfo = JsonSerializer.Deserialize<SignalRConnectionInfoResponse>(json)!;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(connectionInfo.Url, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(connectionInfo.AccessToken);
            })
        .WithAutomaticReconnect()
        .Build();

        _hubConnection.On<string, string>("twitchEvent", (eventType, jsonEvent) =>
        {
            using var doc = JsonDocument.Parse(jsonEvent);
            Timer.HandleTwitchEvent(eventType, doc.RootElement);
        });

        await _hubConnection.StartAsync();
    }

    private void HandleTwitchEvent(string eventType, string eventJson)
    {
        // Process Twitch event JSON into strongly typed model if needed

        switch (eventType)
        {
            case "channel.subscribe":
            case "channel.resubscribe":
                _remaining += TimeSpan.FromMinutes(1); // Example
                break;
            case "channel.cheer":
                _remaining += TimeSpan.FromSeconds(15); // Example
                break;
            case "channel.gift":
                _remaining += TimeSpan.FromSeconds(30);
                break;
                // Add more Twitch event types as needed
        }

        StateHasChanged();
    }
}