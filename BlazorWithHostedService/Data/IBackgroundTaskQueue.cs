using BlazorWithHostedService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BlazorWithHostedService.Data
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(GetQuoteModel workItem);

        ValueTask<GetQuoteModel> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<GetQuoteModel> _quotequeue;

        public BackgroundTaskQueue(int capacity)
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
            _quotequeue = Channel.CreateBounded<GetQuoteModel>(options);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(
            GetQuoteModel workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            await _quotequeue.Writer.WriteAsync(workItem);
        }

        public async ValueTask<GetQuoteModel> DequeueAsync(CancellationToken cancellationToken)
        {
            var workItem = await _quotequeue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }
    }
}
