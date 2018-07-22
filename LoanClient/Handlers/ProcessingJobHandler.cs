using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

namespace LoanClient.Handlers
{
    public class ProcessingJobHandler : IHandleMessages<EventStarted>
    {
        static ILog _log = LogManager.GetLogger<ProcessingJobHandler>();
        public Task Handle(EventStarted message, IMessageHandlerContext context)
        {
            _log.Info("At ProcessingJobHandler");

            var loanEventProcessingDone = new LoanEndpointJobDone
            {
                EventId = message.EventId,
                ErrorMessage = string.Empty
            };

            Thread.Sleep(1000);

            try
            {
                loanEventProcessingDone.FinishedSuccesfullyBit = true;
            }
            catch (Exception ex)
            {
                loanEventProcessingDone.FinishedSuccesfullyBit = false;
                loanEventProcessingDone.ErrorMessage = ex.ToString();
            }

            return context.Publish(loanEventProcessingDone);

        }
    }
}
