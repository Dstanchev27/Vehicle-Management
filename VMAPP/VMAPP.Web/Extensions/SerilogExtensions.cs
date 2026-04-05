namespace VMAPP.Web.Extensions
{
    using Serilog;
    using Serilog.Events;
    using Serilog.Sinks.MSSqlServer;

    using System.Collections.ObjectModel;
    using System.Data;

    using ILogger = Serilog.ILogger;

    public static class SerilogExtensions
    {
        public static ILogger CreateLogger(IConfiguration configuration)
        {
            Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var sinkOptions = new MSSqlServerSinkOptions
            {
                TableName = "Logs",
                SchemaName = "dbo",
                AutoCreateSqlTable = true
            };

            var columnOptions = new ColumnOptions();

            columnOptions.Store.Remove(StandardColumn.Properties);
            columnOptions.Store.Add(StandardColumn.LogEvent);

            columnOptions.Message.DataLength = -1;
            columnOptions.LogEvent.ExcludeAdditionalProperties = true;

            columnOptions.AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn
                {
                    ColumnName = "Username",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 200,
                    AllowNull = true
                }
            };

            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .WriteTo.Console()
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: sinkOptions,
                    columnOptions: columnOptions)
                .CreateLogger();
        }
    }
}
