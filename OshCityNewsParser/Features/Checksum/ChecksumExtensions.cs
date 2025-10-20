using Microsoft.Extensions.DependencyInjection;

namespace OshCityNewsParser.Features.Checksum
{
    internal static class ChecksumExtensions
    {
        internal static IServiceCollection AddChecksum(this IServiceCollection services)
        {
            services.AddScoped<IChecksumService, SHA256ChecksumService>();

            return services;
        }
    }
}