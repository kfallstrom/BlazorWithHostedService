using BlazorWithHostedService.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWithHostedService.Data
{
    public class CacheWorker : BackgroundService
    {
        private readonly ILogger<CacheWorker> _logger;
        public IBackgroundTaskQueue _taskQueue { get; }
        public IBlobQuoteClient _quoteClient;
        public CacheWorker(IBackgroundTaskQueue taskQueue, IBlobQuoteClient quoteClient,
            ILogger<CacheWorker> logger)
        {
            _quoteClient = quoteClient;
            _taskQueue = taskQueue;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                    _logger.LogInformation(
            $"Queued Hosted Service is running.{Environment.NewLine}" +
            $"{Environment.NewLine}Tap W to add a work item to the " +
            $"background queue.{Environment.NewLine}");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem =
                    await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    using (var stream = new MemoryStream())
                    {
                        var payload = Encoding.UTF8.GetBytes($"{workItem.ConnectionId},{workItem.Name},{workItem.QuoteId}");

                        await stream.WriteAsync(payload);
                        stream.Position = 0;
                        await _quoteClient.AddBlob(stream, workItem.QuoteId, workItem.Name);
                    }
                    _logger.LogInformation(workItem.Name, workItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
