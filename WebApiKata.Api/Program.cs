using WebApiKata.Api.IoC;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace WebApiKata.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseLightInject(services => services.RegisterFrom<CompositionRoot>())
                .ConfigureLogging((hostingContext, logging) =>
                {
                    var appInsightKey = hostingContext.Configuration["ApplicationInsights:Instrumentationkey"];
                    logging.AddApplicationInsights(appInsightKey);
                    logging.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Debug);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
