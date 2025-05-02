using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace TheOmenDen.TwitchInterop.Services;

public interface ITwitchUserMappingService
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task StoreMappingAsync(string broadcasterUserId, string azureUserId, CancellationToken cancellationToken = default);
    Task<string?> GetAzureUserIdAsync(string broadcasterUserId, CancellationToken cancellationToken = default);
}

public sealed class TwitchUserMappingService : ITwitchUserMappingService
{
    private readonly TableClient _tableClient;
    private readonly ILogger<TwitchUserMappingService> _logger;

    private const string PartitionKey = "TwitchUser";

    public TwitchUserMappingService(IConfiguration config, ILogger<TwitchUserMappingService> logger)
    {
        var connectionString = config["AzureWebJobsStorage"]
                               ?? config["StorageConnection"] ?? throw new InvalidOperationException("Missing AzureWebJobsStorage");

        _tableClient = new TableClient(connectionString, "TwitchUserMappings");
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _tableClient.CreateIfNotExistsAsync(cancellationToken);
    }

    public async Task StoreMappingAsync(string broadcasterUserId, string azureUserId, CancellationToken cancellationToken = default)
    {
        var entity = new TableEntity(PartitionKey, broadcasterUserId)
        {
            { "AzureUserId", azureUserId }
        };

        await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken);
        _logger.LogInformation("Stored mapping: {BroadcasterId} → {AzureUserId}", broadcasterUserId, azureUserId);
    }

    public async Task<string?> GetAzureUserIdAsync(string broadcasterUserId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(broadcasterUserId))
            throw new ArgumentException("Broadcaster ID is required", nameof(broadcasterUserId));

        var response = await _tableClient.GetEntityIfExistsAsync<TableEntity>(
            partitionKey: "TwitchUser",
            rowKey: broadcasterUserId,
            cancellationToken: cancellationToken);

        if (!response.HasValue)
        {
            _logger.LogWarning("No user mapping found for broadcaster_user_id: {BroadcasterId}", broadcasterUserId);
            return null;
        }

        var entity = response.Value;
        return entity.GetString("AzureUserId");
    }

}
