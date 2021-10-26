using BlazorWithHostedService.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWithHostedService.Hubs
{
    public class BlobUploadedHub:Hub
    {
     
        public async Task NotifyBlobSuccess(GetQuoteModelResponse message )
        {
            await Clients.All.SendAsync("updateUI",message);
        }
    }
}
