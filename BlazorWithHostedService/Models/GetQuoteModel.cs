using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWithHostedService.Models
{
    public class GetQuoteModel:TaskFromUser
    {
        public GetQuoteModel():base() { }
        public string QuoteId { get; set; }
        public string Name { get; set; }
    }
}
