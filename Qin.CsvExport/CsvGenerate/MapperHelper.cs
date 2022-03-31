namespace Qin.CsvRelevant
{
    using System;
    using System.Linq.Expressions;

    public static class MapperHelper
    {
        public static Mapper Map<T>(this ICsvGenerate csvGenerate, string columnName, Expression<Func<T, object>> propertySelector)
        {
            return new Mapper().Map<T>(columnName, propertySelector);
        }
    }
}
