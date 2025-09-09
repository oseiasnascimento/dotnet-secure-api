using Guardian.Application.Shared.Contracts;
using Guardian.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace Guardian.Infrastructure.Shared
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailService, EmailService>();
            services.AddTransient<ILoggerService, LoggerService>();

            #region Serilog Settings

            // string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            const string tableName = "LOGS";

            ColumnOptions columnOptions = new()
            {
                AdditionalColumns = [
                    new() { ColumnName = "Action" },
                    new() { ColumnName = "UserId" }
                ]
            };

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new()
                    {
                        AutoCreateSqlTable = true,
                        TableName = tableName
                    },
                    columnOptions: columnOptions
                ).CreateLogger();

            #endregion

            //services.AddScoped<IUploadFileService, UploadFileService>();

            return services;
        }
    }
}
