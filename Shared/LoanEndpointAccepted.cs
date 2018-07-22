using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

namespace Shared
{
    public class LoanEndpointAccepted :
    //IEvent
        IMessage
    {
        public Guid EventId { get; set; }
        public Guid LoanId { get; set; }
        public bool AcceptedBit { get; set; }
    }
}
