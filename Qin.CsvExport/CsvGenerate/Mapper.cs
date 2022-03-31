namespace Qin.CsvRelevant
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public class Mapper
    {
        private readonly Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        public Mapper Map<T>(string columnName, Expression<Func<T, object>> propertySelector)
        {
            var propertyinfo = GetPropertyInfoByExpression(propertySelector);
            if (propertyinfo == null)
                throw new ArgumentException("Attribute mapping failed!", nameof(propertyinfo));
            _dictionary.Add(columnName, propertyinfo.Name);
            return this;
        }

        private PropertyInfo GetPropertyInfoByExpression<T>(Expression<Func<T, object>> propertySelector)
        {
            var expression = propertySelector as LambdaExpression;

            if (expression == null)
                throw new ArgumentException("Only LambdaExpression is allowed!", nameof(propertySelector));

            var body = expression.Body.NodeType == ExpressionType.MemberAccess ?
                (MemberExpression)expression.Body :
                (MemberExpression)((UnaryExpression)expression.Body).Operand;

            // body.Member will return the MemberInfo of base class, so we have to get it from T...
            //return (PropertyInfo)body.Member;
            return typeof(T).GetMember(body.Member.Name)[0] as PropertyInfo;
        }

        public Dictionary<string, string> BuildDictionary() { return _dictionary; }
    }
}
