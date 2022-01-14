using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            //create a scope which is going to host any services that we create inside this particular method.
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                //using service locator pattern using GetRequiredService to use DataContext
                var context = services.GetRequiredService<DataContext>();
                //apply any pending migration from a context to the database
                //it will create a database using migration if we don't have it 
                await context.Database.MigrateAsync();
                // To seed data into db if db has no data
                await Seed.SeedData(context);
            }
            catch(Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex,"An error occured during migration.");
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
