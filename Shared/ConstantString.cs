using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class ConstantString
    {
        public static string ConnectionString = @"Data Source=EC2AMAZ-2HI57V7;Initial Catalog=Oura;Integrated Security=True";
        public static string SagaEndpoint = "SagaEndpoint";
        public static string LoanWorkerEndpoint = "LoanWorkerEndpoint";
        public static string PersistenceLoanClientPrefix = "lc";
        public static string PersistenceSagaPrefix = "sg";
        public static string SagaErrorTableName = "sagaError";
        public static string SagaAuditTableName = "sagaAudit";
        public static string SchemaName = "msg";
        public static string LoanClientErrorTableName = "lcError";
        public static string LoanClientAuditTableName = "lcAudit";
        public static string ErrorTableName = "Error";
        public static string AuditTableName = "Audit";
    }
}

