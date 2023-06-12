namespace Infrastructure.Services.Background
{
    using System.Threading.Channels;

    using Application.Interfaces.Services;

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, Task>> _queue;

        public BackgroundTaskQueue()
        {
            _queue = Channel.CreateUnbounded<Func<CancellationToken, Task>>();
        }

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _queue.Writer.TryWrite(workItem);
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            var workItem = await _queue.Reader.ReadAsync(cancellationToken);
            return workItem;
        }
    }
}