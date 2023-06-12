namespace Application.Interfaces.Services
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}