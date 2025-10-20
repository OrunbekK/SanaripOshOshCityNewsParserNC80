using Microsoft.Extensions.DependencyInjection;

namespace OshCityNewsParser.Features.Normalization
{
    internal static class NormalizationExtensions
    {
        internal static IServiceCollection AddNormalization(this IServiceCollection services)
        {
            services.AddScoped<INormalizeService, TextNormalizer>();

            return services;
        }
    }
}