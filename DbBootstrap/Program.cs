using System;
using Shared;

namespace DbBootstrap
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlHelper.CreateSchema(ConstantString.ConnectionString, ConstantString.SchemaName);
        }
    }
}
