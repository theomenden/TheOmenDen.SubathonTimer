namespace TheOmenDen.SubathonTimer.Services.EventBus.Execution;

public interface IExecutionStrategy
{
    Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken token);
}