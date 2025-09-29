using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using Guardian.Infrastructure.Data.Contexts;

namespace WebApi.Helpers
{
    public static class AutoMigrateHelper
    {
        private static bool DatabaseIsReady(IConfiguration configuration)
        {
            string databaseAddress = configuration["DatabaseSettings:Server"];
            int databasePort = int.Parse(configuration["DatabaseSettings:Port"]);

            int timeoutInMinutes = 1;

            DateTime startTime = DateTime.Now;

            while (DateTime.Now < startTime.AddMinutes(timeoutInMinutes))
            {
                try
                {
                    Console.WriteLine("...Testing...");
                    using TcpClient client = new();
                    client.Connect(databaseAddress, databasePort);

                    if (client.Connected)
                    {
                        Console.WriteLine("Database is ready!");
                        return true;
                    }
                }
                catch
                {
                    Console.WriteLine("Database is not ready!\n...Waiting 3 seconds before try again...");
                    Thread.Sleep(3000);
                }
            }

            throw new TimeoutException("Timeout exceeded. Database Problems.");
        }

        public static void Migrate(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (DatabaseIsReady(configuration))
            {
                using IServiceScope serviceScope = app.ApplicationServices.CreateScope();
                serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();
            }
            else
            {
                throw new Exception("Problema no banco de dados. Verifique e tente novamente.");
            }
        }
    }
}