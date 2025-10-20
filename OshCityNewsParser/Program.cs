using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OshCityNewsParser.Logging;
using OshCityNewsParser.Configuration;
using Serilog;
using OshCityNewsParser.Persistence;
using OshCityNewsParser.Features.Fetching;

namespace OshCityNewsParser
{
    internal class Program
    {
        internal static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                // Configuration Binding
                services.Configure<ParserOptions>(context.Configuration.GetSection("ParserSettings"));
                services.Configure<DatabaseOptions>(context.Configuration.GetSection("ConnectionStrings"));
                services.Configure<LoggingOptions>(context.Configuration.GetSection("Serilog"));

                // Logging
                LoggingExtensions.ConfigureSerilog(context.Configuration);

                // Features (будут добавлены в PHASE 4-8)
                services.AddFetching();
                // services.AddParsing();
                // services.AddChecksum();
                // services.AddNormalization();
                // services.AddProcessing();

                services.AddPersistence();

                // Workers (будет добавлено в PHASE 9)
                // services.AddHostedService<NewsParserWorker>();

                // Health Checks (будет добавлено в PHASE 10)
                // services.AddHealthChecks();
            })
            .UseSerilog();

            var host = builder.Build();
            await host.RunAsync();
        }
    }
}