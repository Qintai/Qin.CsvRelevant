namespace Qin.CsvRelevant
{
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class CsvRelevantExtend
    {
        private static IServiceCollection AddCsvGenerateCore(this IServiceCollection services, 
            ServiceLifetime lifetime)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            
            services.Add(new ServiceDescriptor(
                typeof(ICsvGenerate), 
                typeof(CsvGenerateDefault), 
                lifetime));
                
            return services;
        }

        public static IServiceCollection AddCsvGenerate(this IServiceCollection services)
            => AddCsvGenerateCore(services, ServiceLifetime.Singleton);

        public static IServiceCollection AddScopedByCsvGenerate(this IServiceCollection services)
            => AddCsvGenerateCore(services, ServiceLifetime.Scoped);

        public static IServiceCollection AddTransientByCsvGenerate(this IServiceCollection services)
            => AddCsvGenerateCore(services, ServiceLifetime.Transient);
    }
}