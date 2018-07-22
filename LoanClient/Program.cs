using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using NServiceBus.Transport.SQLServer;
using Shared;

namespace LoanClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Loan Worker";
            var endpointConfiguration = new EndpointConfiguration(ConstantString.LoanWorkerEndpoint);
            endpointConfiguration.SendFailedMessagesTo(ConstantString.ErrorTableName);
            endpointConfiguration.AuditProcessedMessagesTo(ConstantString.AuditTableName);
            endpointConfiguration.EnableInstallers();

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(ConstantString.ConnectionString);
            transport.DefaultSchema(ConstantString.SchemaName);
            transport.UseSchemaForQueue(ConstantString.ErrorTableName, ConstantString.SchemaName);
            transport.UseSchemaForQueue(ConstantString.AuditTableName, ConstantString.SchemaName);
            transport.UseSchemaForEndpoint(ConstantString.SagaEndpoint, ConstantString.SchemaName);
            //transport.CreateMessageBodyComputedColumn();

            var routing = transport.Routing();
            routing.RegisterPublisher(typeof(EventStarted).Assembly, ConstantString.SagaEndpoint);
            routing.RouteToEndpoint(typeof(LoanEndpointAccepted),ConstantString.SagaEndpoint);
            //routing.RouteToEndpoint(typeof(LoanEndpointJobDone), ConstantString.SagaEndpoint);


            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>();
            dialect.Schema(ConstantString.SchemaName);
            persistence.ConnectionBuilder(() => new SqlConnection(ConstantString.ConnectionString));
            persistence.TablePrefix(ConstantString.PersistenceLoanClientPrefix);

            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));

            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press Esc to exit");
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
            await endpointInstance.Stop().ConfigureAwait(false);
        }
    }
}
