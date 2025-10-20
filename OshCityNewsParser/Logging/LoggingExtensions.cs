using Microsoft.Extensions.Configuration;
using Serilog;

namespace OshCityNewsParser.Logging
{
    internal static class LoggingExtensions
    {
        internal static void ConfigureSerilog(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .CreateLogger();

            Log.Information("Serilog configured successfully");
        }
    }
}