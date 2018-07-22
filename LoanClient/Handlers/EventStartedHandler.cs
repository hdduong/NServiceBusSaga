using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

namespace LoanClient.Handlers
{
    public class EventStartedHandler : IHandleMessages<EventStarted>
    {
        static ILog _log = LogManager.GetLogger<EventStartedHandler>();

        public Task Handle(EventStarted message, IMessageHandlerContext context)
        {
            _log.Info("At EventStartedHandler");

            var loanEventAccepted = new LoanEndpointAccepted
            {
                EventId = message.EventId,
                AcceptedBit = true
            };
            //return context.Publish(loanEventAccepted);
            return context.Reply(loanEventAccepted);
        }
    }
}
