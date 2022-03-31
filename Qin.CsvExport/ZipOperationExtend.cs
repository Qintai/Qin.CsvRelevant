namespace Qin.CsvRelevant
{
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class ZipOperationExtend
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
    }
}