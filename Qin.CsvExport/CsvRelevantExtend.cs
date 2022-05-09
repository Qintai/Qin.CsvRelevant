namespace Qin.CsvRelevant
{
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class CsvRelevantExtend
    {
        public static IServiceCollection AddCsvGenerate(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }
            services.AddSingleton<ICsvGenerate, CsvGenerateDefault>();
            return services;
        }

        public static IServiceCollection AddScopedByCsvGenerate(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }
            services.AddScoped<ICsvGenerate, CsvGenerateDefault>();
            return services;
        }

        public static IServiceCollection AddTransientByCsvGenerate(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }
            services.AddTransient<ICsvGenerate, CsvGenerateDefault>();
            return services;
        }
    }
}