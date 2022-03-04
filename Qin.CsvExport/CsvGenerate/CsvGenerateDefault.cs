namespace Qin.CsvRelevant
{
    using Sylvan.Data.Csv;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    internal class CsvGenerateDefault : ICsvGenerate
    {
        public byte[] Write<T>(List<T> listData, Dictionary<string, string> column, string fileName = "", Func<string, object, object> propOperation = null)
        {
            if (listData == null || column == null)
            {
                throw new AggregateException("Parameter cannot be null");
            }

            StringBuilder stringbuilder = BuildStringBuilder(listData, column, propOperation);

            char[] charArr = new char[stringbuilder.Length];
            stringbuilder.CopyTo(0, charArr, 0, stringbuilder.Length);

            byte[] charBytes = Encoding.Default.GetBytes(charArr);
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                using Stream stream = new MemoryStream(charBytes);
                using StreamReader streamReader = new StreamReader(stream);

                CsvDataReader csvcra = CsvDataReader.Create(streamReader);
                using var csvdatawriter = CsvDataWriter.Create(fileName);
                csvdatawriter.Write(csvcra);
            }

            return charBytes;
        }

        public byte[] WriteByAttribute<T>(List<T> listData, string fileName = "", Func<string, object, object> propOperation = null)
        {
            if (listData == null)
            {
                throw new AggregateException("Parameter cannot be null");
            }

            Dictionary<string, string> column = ToExcelComumn<T>();
            return Write(listData, column, fileName, propOperation);
        }

        public async Task<byte[]> WriteAsync<T>(List<T> listData, Dictionary<string, string> column, string fileName = "", Func<string, object, object> propOperation = null)
        {
            var bytearr = await Task.Run(() =>
            {
                return Write<T>(listData, column, fileName, propOperation);
            });
            return bytearr;
        }

        public async Task<byte[]> WriteByAttributeAsync<T>(List<T> listData, string fileName = "", Func<string, object, object> propOperation = null)
        {
            var bytearr = await Task.Run(() =>
            {
                return WriteByAttribute<T>(listData, fileName, propOperation);
            });
            return bytearr;
        }

        public StringBuilder BuildStringBuilder<T>(List<T> list, Dictionary<string, string> column, Func<string, object, object> propOperation = null)
        {
            List<object> strch = new List<object>();
            StringBuilder builder = new StringBuilder();
            string[] columns = column.Keys.Select(s => new string(s)).ToArray();
            builder.AppendJoin<string>(',', columns);
            builder.Append("\n");
            Type type = null;
            if (list.Count > 0) type = list[0].GetType();

            foreach (var item in list)
            {
                foreach (var i in column)
                {
                    var prop = type.GetProperty(i.Value);
                    if (prop == null)
                        throw new Exception($"There is no {i.Value} attribute in the {type.Name} class. Please check whether the attribute is written incorrectly");

                    object fieldvalue = prop.GetValue(item);

                    if (_forMat != null)
                         fieldvalue = _forMat(i.Key, i.Value, fieldvalue);

                    if (propOperation != null)
                        fieldvalue = propOperation(i.Value, fieldvalue);

                    strch.Add(fieldvalue);
                }

                builder.AppendJoin<object>(',', strch);
                builder.Append("\n");
                strch.Clear();
            }

            return builder;
        }

        private Func<string, string, object, object> _forMat = (column, fieldname, fieldvalue) => 
        {
            if (fieldvalue !=null && fieldvalue is DateTime date)
                fieldvalue = date.ToString("yyyy-MM-dd HH:mm:ss");
            return fieldvalue;
        };

        Func<string, string, object, object> ICsvGenerate.ForMat { get => _forMat; set => _forMat = value; }

        public Dictionary<string, string> ToExcelComumn<T>()
        {
            Type type = typeof(T);
            Type column = typeof(ExportColumnAttribute);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (PropertyInfo item in type.GetProperties())
            {
                var model = item.GetCustomAttributes(column, true).Where(it => it is ExportColumnAttribute).SingleOrDefault() as ExportColumnAttribute;
                if (model != null)
                    dic.Add(model.ExcelColumnName, item.Name);
            }
            return dic;
        }
    }
}
