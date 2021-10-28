using BlazorWithHostedService.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWithHostedService.Hubs
{
    public interface IBlobHub
    {
        Task NotifyBlobSuccess(GetQuoteModelResponse message);
    }
    public class BlobUploadedHub:Hub<IBlobHub>
    {
     
        public async Task NotifyBlobSuccess(GetQuoteModelResponse message )
        {
            await Clients.Client(message.ConnectionId).NotifyBlobSuccess(message);
        }
    }
}
