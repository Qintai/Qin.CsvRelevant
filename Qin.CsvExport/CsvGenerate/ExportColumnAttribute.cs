using System;

namespace Qin.CsvRelevant
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ExportColumnAttribute : Attribute
    {
        public string ExcelColumnName { get; }

        public ExportColumnAttribute(string name)
        {
            ExcelColumnName = name;
        }

    }
}

