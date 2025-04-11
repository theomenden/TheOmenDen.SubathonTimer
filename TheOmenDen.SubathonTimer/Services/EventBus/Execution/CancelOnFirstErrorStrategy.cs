namespace TheOmenDen.SubathonTimer.Services.EventBus.Execution;

public sealed class CancelOnFirstErrorStrategy : IExecutionStrategy
{
    public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken token)
    {
        await action(token); // Let the caller handle cancellation or bubble errors
    }
}