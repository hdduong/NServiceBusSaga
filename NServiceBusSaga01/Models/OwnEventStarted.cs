using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;
using Shared;

namespace NServiceBusSaga01.Models
{
    public class OwnEventStarted 
        : IMessage
    {
        public Guid EventId { get; set; }
        public Guid LoanId { get; set; }
        public OwnEventStarted(EventStarted anEvent)
        {
            EventId = anEvent.EventId;
            LoanId = anEvent.LoanId;
        }
    }

    
}
