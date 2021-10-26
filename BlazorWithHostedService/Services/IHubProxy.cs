using BlazorWithHostedService.Services;
using BlazorWithHostedService.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuidantFinancial.ClientHub.Services
{
    public interface IHubProxy
    {
        public void BuildConnection();

        public Task StartAsync(CancellationToken cancellationToken = default);

        public Task StopAsync(CancellationToken cancellationToken = default);

        public ValueTask DisposeAsync();

        public IDisposable On<T>(string methodName, Action<T> handler);

        public Task AddToGroup(string groupName, CancellationToken cancellationToken = default);

        public Task SendAsync(string methodName, object? arg1, CancellationToken cancellationToken = default);

        public HubConnectionState State { get; }
    }

    public class HubProxy : IHubProxy
    {
        private readonly IHubConnectionProxy _hubConnectionProxy;
        private readonly BlobUploadedHub _hub;

        public HubProxy(IHubConnectionProxy hubConnectionProxy, BlobUploadedHub hub)
        {
            _hubConnectionProxy = hubConnectionProxy;
            _hub = hub;
        }

        public HubConnectionState State => _hubConnectionProxy.State();

        public void BuildConnection()
        {
            _hubConnectionProxy.Build("blobupload");
        }

        public virtual async Task AddToGroup(string groupName, CancellationToken cancellationToken = default)
        {
            await _hub.Groups.AddToGroupAsync(_hubConnectionProxy.ConnectionId(), groupName, cancellationToken);
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _hubConnectionProxy.StartAsync(cancellationToken);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _hubConnectionProxy.StopAsync(cancellationToken);
        }

        public virtual async ValueTask DisposeAsync()
        {
            await _hubConnectionProxy.DisposeAsync();
        }

        public IDisposable On<T>(string methodName, Action<T> handler)
        {
            return _hubConnectionProxy.On<T>(methodName, handler);
        }

        public async Task SendAsync(string methodName, object arg1, CancellationToken cancellationToken = default)
        {
            await _hubConnectionProxy.SendAsync(methodName, arg1, cancellationToken);
        }
    }
}