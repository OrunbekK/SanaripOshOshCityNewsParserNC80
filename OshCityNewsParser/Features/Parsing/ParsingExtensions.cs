using Microsoft.Extensions.DependencyInjection;

namespace OshCityNewsParser.Features.Parsing
{
    internal static class ParsingExtensions
    {
        internal static IServiceCollection AddParsing(this IServiceCollection services)
        {
            services.AddSingleton<SelectorsProvider>();
            services.AddScoped<IParsersService, HtmlParser>();

            return services;
        }
    }
}