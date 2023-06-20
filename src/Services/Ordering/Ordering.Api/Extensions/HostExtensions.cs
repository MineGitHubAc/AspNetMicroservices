using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Ordering.Api.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, 
            Action<TContext, IServiceProvider> seeder, int? retry = 0) where TContext : DbContext
        {
            int retryForAvailability = retry.Value;
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();
                try
                {
                    logger.LogInformation("migration started");
                    InvokeSeeder(seeder,context,services);
                    logger.LogInformation("migration ended");
                }
                catch (SqlException exc)
                {
                    logger.LogError("Error occured while migration");
                    if (retryForAvailability < 5)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host,seeder, retryForAvailability);
                    }
                }
            }
            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, 
            TContext context,IServiceProvider services) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context,services);
        }
    }
}
