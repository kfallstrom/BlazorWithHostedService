using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWithHostedService.Models
{
    public abstract class TaskFromUser
    {
        public string ConnectionId { get; set; }
    }
}
