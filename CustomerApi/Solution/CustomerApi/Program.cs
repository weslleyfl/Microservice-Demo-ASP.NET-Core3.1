using CustomerApi.Data.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CustomerApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build() //.MigrateDatabase()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((hostingcontext, config) =>
                 {
                     config.AddJsonFile("appsettings.json", false, true)
                         .AddJsonFile($"appsettings.{hostingcontext.HostingEnvironment.EnvironmentName}.json", true, true);
                 })
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseStartup<Startup>();
                 });
    }
}
