using Microsoft.Extensions.DependencyInjection;
using OshCityNewsParser.Persistence.StoredProcedures;

namespace OshCityNewsParser.Persistence
{
    internal static class PersistenceExtensions
    {
        internal static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
            services.AddScoped<INewsRepository, MsSqlNewsRepository>();
            services.AddScoped<ISettingsRepository, MsSqlSettingsRepository>();

            return services;
        }
    }
}