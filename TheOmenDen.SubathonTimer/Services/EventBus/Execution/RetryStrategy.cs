namespace TheOmenDen.SubathonTimer.Services.EventBus.Execution;

public sealed class RetryStrategy(ILogger<RetryStrategy> logger, int maxRetries = 3) : IExecutionStrategy
{
    public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken token)
    {
        var attempt = 0;
        while (attempt < maxRetries)
        {
            try
            {
                await action(token);
                return;
            }
            catch (Exception ex) when (attempt < maxRetries - 1)
            {
                attempt++;
                logger.LogWarning(ex, "Retry {Attempt}/{Max} failed.", attempt, maxRetries);
                if (attempt >= maxRetries) throw;
                await Task.Delay(200 * attempt, token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Max retries reached. Giving up.");
                break;
            }
        }
    }
}