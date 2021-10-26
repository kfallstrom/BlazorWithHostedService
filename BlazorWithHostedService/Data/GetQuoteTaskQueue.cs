using BlazorWithHostedService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BlazorWithHostedService.Data
{
    /// <summary>
    /// provides a bounded Queue for a type of producer/consumer = GetQuoteModel
    /// </summary>
    public class GetQuoteTaskQueue : BackgroundMessageTaskQueue<GetQuoteModel>
    {
        public GetQuoteTaskQueue():base(100) {
        }
    }
}
