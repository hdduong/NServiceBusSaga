using System;
using System.Data;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;
using NServiceBusSaga01.Models;
using Shared;

namespace NServiceBusSaga01
{
    public class EventLifecycleSaga : SqlSaga<EventSagaData>,
        IAmStartedByMessages<OwnEventStarted>,
        IHandleMessages<LoanEndpointAccepted>,
        IHandleMessages<LoanEndpointJobDone>,
        IHandleTimeouts<EventTimeout>
    {
        protected override void ConfigureMapping(IMessagePropertyMapper mapper)
        {
            mapper.ConfigureMapping<OwnEventStarted>(_ => _.EventId);
            mapper.ConfigureMapping<LoanEndpointAccepted>(_ => _.EventId);
            mapper.ConfigureMapping<LoanEndpointJobDone>(_ => _.EventId);
        }

        protected override string CorrelationPropertyName => nameof(EventSagaData.EventId);

        static ILog _log = LogManager.GetLogger<EventLifecycleSaga>();

        public Task Handle(OwnEventStarted message, IMessageHandlerContext context)
        {
            _log.Info("Handle(EventStarted message, IMessageHandlerContext context)");

            Data.EventId = message.EventId;
            Data.LoanId = message.LoanId;
            Data.SagaStartTimeUtc = DateTime.UtcNow;
            Data.LoanEndpointAcceptedBit = false;
            Data.LoanEndpointDoneBit = false;
            Data.LoanEndpointErrorMsg = string.Empty;

            return CreateTimeoutRequest(context);
        }

        public Task CreateTimeoutRequest(IMessageHandlerContext context)
        {
            return RequestTimeout<EventTimeout>(context, TimeSpan.FromSeconds(60));
        }

        public Task Handle(LoanEndpointAccepted message, IMessageHandlerContext context)
        {
            _log.Info("Handle(LoanEndpointAccepted message, IMessageHandlerContext context)");
            Data.EventId = message.EventId;
            Data.LoanId = message.LoanId;
            Data.LoanEndpointAcceptedBit = message.AcceptedBit;
            _log.Info("Done LoanEndpointAccepted");
            return CreateTimeoutRequest(context);
        }

        public Task Timeout(EventTimeout state, IMessageHandlerContext context)
        {          
            if (!Data.LoanEndpointDoneBit && DateTime.UtcNow < Data.SagaStartTimeUtc.AddMinutes(60))
            {
                return RequestTimeout<EventTimeout>(context, TimeSpan.FromSeconds(60));
            }
            MarkAsComplete();
            return Task.CompletedTask;
        }

        public Task Handle(LoanEndpointJobDone message, IMessageHandlerContext context)
        {
            _log.Info("Handle(LoanEndpointJobDone message, IMessageHandlerContext context)");

            Data.EventId = message.EventId;
            Data.LoanId = message.LoanId;
            Data.LoanEndpointDoneBit = message.FinishedSuccesfullyBit;
            if (!Data.LoanEndpointDoneBit)
            {
                Data.LoanEndpointErrorMsg = message.ErrorMessage;
            }

            MarkAsComplete();
            return Task.CompletedTask;
        }


      
    }
}

