using Microsoft.Extensions.DependencyInjection;

namespace OshCityNewsParser.Features.Fetching
{
    internal static class FetchingExtensions
    {
        internal static IServiceCollection AddFetching(this IServiceCollection services)
        {
            services.AddScoped<IHttpFetcher, HttpFetcher>();

            return services;
        }
    }
}