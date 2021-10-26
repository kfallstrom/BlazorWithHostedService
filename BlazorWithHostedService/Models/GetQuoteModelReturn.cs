using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWithHostedService.Models
{
    public class GetQuoteModelReturn:GetQuoteModel
    {
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
