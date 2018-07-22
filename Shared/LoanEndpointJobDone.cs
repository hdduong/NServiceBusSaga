using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

namespace Shared
{
    public class LoanEndpointJobDone : IEvent
    {
        public Guid EventId { get; set; }
        public Guid LoanId { get; set; }
        public bool FinishedSuccesfullyBit { get; set; }
        public string ErrorMessage { get; set; }
    }
}
