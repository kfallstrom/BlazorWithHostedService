using BlazorWithHostedService.Hubs;
using BlazorWithHostedService.Models;
using BlazorWithHostedService.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
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
    public class QuoteBlobBackgroundService : BackgroundService
    {
        private readonly ILogger<QuoteBlobBackgroundService> _logger;
        public IBackgroundMessageTaskQueue<GetQuoteModel> _taskQueue { get; }
        public IBlobQuoteClient _quoteClient;
        public IHubContext<BlobUploadedHub,IBlobHub> _hubContext;
        public IServiceProvider _services;
        public QuoteBlobBackgroundService(IBackgroundMessageTaskQueue<GetQuoteModel> taskQueue, IBlobQuoteClient quoteClient, IServiceProvider serviceProvider,IHubContext<BlobUploadedHub, IBlobHub> hubContext,
            ILogger<QuoteBlobBackgroundService> logger)
        {
            _services = serviceProvider;
            _hubContext = hubContext;
            _quoteClient = quoteClient;
            _taskQueue = taskQueue;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                    _logger.LogInformation(
            $"Queued Hosted Service is running.");

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
                        var rnd = new Random();
                        for (var i = 0; i < rnd.Next(50000,100000); i++)
                        {
                            await stream.WriteAsync(payload);
                        }
                        stream.Position = 0;
                        var result = await _quoteClient.AddBlob(stream, workItem.QuoteId, workItem.Name);
                        
                        result.ConnectionId = workItem.ConnectionId;
                        await _hubContext.Clients.All.NotifyBlobSuccess(result);
                        
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
