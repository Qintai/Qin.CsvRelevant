using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Qin.CsvRelevant
{
    public static class MapperHelper
    {
        public static Mapper Map<T>(this ICsvGenerate csvGenerate, string columnName, Expression<Func<T, object>> propertySelector)
        {
            Mapper mapper = new Mapper();
            return mapper.Map<T>(columnName, propertySelector);
        }
    }
}
