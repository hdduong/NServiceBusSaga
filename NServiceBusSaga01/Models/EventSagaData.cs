using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

namespace Shared
{
    public class EventSagaData : IContainSagaData
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
        public Guid EventId { get; set; }
        public Guid LoanId { get; set; }
        public bool LoanEndpointAcceptedBit { get; set; }
        public bool LoanEndpointDoneBit { get; set; }
        public string LoanEndpointErrorMsg { get; set; }
        public DateTime SagaStartTimeUtc { get; set; }
    }
}
