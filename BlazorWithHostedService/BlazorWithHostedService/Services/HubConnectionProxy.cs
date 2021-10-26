using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWithHostedService.Services
{
    public interface IHubConnectionProxy
    {
        void Build(string url);

        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);

        Task DisposeAsync();

        IDisposable On<T>(string methodName, Action<T> handler);

        HubConnectionState State();

        Task SendAsync(string methodName, object arg1, CancellationToken cancellationToken = default);

        string ConnectionId();
    }

    [ExcludeFromCodeCoverageAttribute]
    public class HubConnectionProxy : IHubConnectionProxy
    {
        private readonly HubConnectionBuilder _hubConnectionBuilder;
        private readonly ILogger<HubConnectionProxy> _logger;
        private HubConnection _hubConnection;

        public HubConnectionProxy(ILogger<HubConnectionProxy> logger)
        {
            _hubConnectionBuilder = new HubConnectionBuilder();
            _logger = logger;
        }

        public string ConnectionId() => _hubConnection.ConnectionId;

        public HubConnectionState State() => _hubConnection.State;

        public void Build(string serverConnection)
        {
            _logger.LogInformation($"SignalR is now connected to {serverConnection}");

            _hubConnection = _hubConnectionBuilder
                .WithUrl(serverConnection)
                .Build();
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _hubConnection.StartAsync(cancellationToken);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _hubConnection.StopAsync(cancellationToken);
        }

        public virtual async Task DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }

        public virtual IDisposable On<T>(string methodName, Action<T> handler)
        {
            return _hubConnection.On<T>(methodName, handler);
        }

        public virtual async Task SendAsync(string methodName, object arg1, CancellationToken cancellationToken = default)
        {
            await _hubConnection.SendAsync(methodName, arg1, default);
        }

    }
}