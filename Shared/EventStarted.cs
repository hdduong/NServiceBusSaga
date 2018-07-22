using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

namespace Shared
{
    public class EventStarted : IEvent
    {
        public Guid EventId { get; set;}
        public Guid LoanId { get; set; }
    }
}
