using System;
using System.Data.SqlClient;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using NServiceBus.Transport.SQLServer;
using NServiceBusSaga01.Models;
using Shared;

namespace NServiceBusSaga01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Distributed Saga Controller";

            var endpointConfiguration = new EndpointConfiguration(ConstantString.SagaEndpoint);
            endpointConfiguration.SendFailedMessagesTo(ConstantString.ErrorTableName);
            endpointConfiguration.AuditProcessedMessagesTo(ConstantString.AuditTableName);
            endpointConfiguration.EnableInstallers();

            
            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(ConstantString.ConnectionString);
            transport.DefaultSchema(ConstantString.SchemaName);
            transport.UseSchemaForQueue(ConstantString.ErrorTableName, ConstantString.SchemaName);
            transport.UseSchemaForQueue(ConstantString.AuditTableName, ConstantString.SchemaName);
            transport.UseSchemaForEndpoint(ConstantString.LoanWorkerEndpoint, ConstantString.SchemaName);
            //transport.CreateMessageBodyComputedColumn();

            var routing = transport.Routing();
            // routing.RegisterPublisher(typeof(LoanEndpointAccepted).Assembly, ConstantString.LoanWorkerEndpoint);
            // routing.RouteToEndpoint(typeof(LoanEndpointAccepted), ConstantString.SagaEndpoint);
            routing.RegisterPublisher(typeof(LoanEndpointJobDone).Assembly, ConstantString.LoanWorkerEndpoint);
            routing.RegisterPublisher(typeof(LoanEndpointAccepted).Assembly, ConstantString.LoanWorkerEndpoint);

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>();
            dialect.Schema(ConstantString.SchemaName);
            persistence.ConnectionBuilder(() => new SqlConnection(ConstantString.ConnectionString));
            persistence.TablePrefix(ConstantString.PersistenceSagaPrefix);

            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));

            // endpointConfiguration.EnableOutbox();

            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            Console.WriteLine("Press any key to send a message");
            Console.WriteLine("Press Esc to exit");

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }

                var loanEventTriggered = new EventStarted
                {
                   EventId = Guid.NewGuid(),
                   LoanId = new Guid("ec9bdd39-15d0-4d54-af1b-d7a39fb35579")
                };
                var ownEvent = new OwnEventStarted(loanEventTriggered);

                await endpointInstance.SendLocal(ownEvent).ConfigureAwait(false);
                Console.WriteLine("Done EndpointInstance.SendLocal(ownEvent).ConfigureAwait(false);");

                await endpointInstance.Publish(loanEventTriggered).ConfigureAwait(false);
                Console.WriteLine("await endpointInstance.Publish(loanEventTriggered).ConfigureAwait(false);");

                Console.WriteLine("Published LoanEventTriggered event: " + JsonConvert.SerializeObject(loanEventTriggered));
            }
            await endpointInstance.Stop().ConfigureAwait(false);

        }
    }
}
