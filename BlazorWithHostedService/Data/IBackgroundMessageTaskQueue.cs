using BlazorWithHostedService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BlazorWithHostedService.Data
{
    public interface IBackgroundMessageTaskQueue<T>
    {
        ValueTask QueueBackgroundWorkItemAsync(T workItem);

        ValueTask<T> DequeueAsync(CancellationToken cancellationToken);
    }

    public abstract class BackgroundMessageTaskQueue<T> : IBackgroundMessageTaskQueue<T> where T : TaskFromUser 
    {
        private readonly Channel<T> _queue;

        public BackgroundMessageTaskQueue(int capacity)
        {
            // Capacity should be set based on the expected application load and
            // number of concurrent threads accessing the queue.            
            // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
            // which completes only when space became available. This leads to backpressure,
            // in case too many publishers/calls start accumulating.
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
            };
            _queue = Channel.CreateBounded<T>(options);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(
            T workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            await _queue.Writer.WriteAsync(workItem);
        }

        public async ValueTask<T> DequeueAsync(CancellationToken cancellationToken)
        {
            var workItem = await _queue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }
    }
}
