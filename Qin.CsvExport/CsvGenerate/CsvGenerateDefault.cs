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
        public CsvDataWriterOptions Options { get; set; }
        public string TimeFormatting { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public Func<string, string, object, object> ForMat { get; set; }
        public bool Stdout { get; set; } = false;

        public byte[] Write<T>(List<T> listData, Dictionary<string, string> column, string fileName = "")
        {
            if (listData == null || column == null)
            {
                throw new AggregateException("Parameter cannot be null");
            }

            StringBuilder stringbuilder = BuildStringBuilder(listData, column);
            char[] charArr = new char[stringbuilder.Length];
            stringbuilder.CopyTo(0, charArr, 0, stringbuilder.Length);
            byte[] charBytes = Encoding.Default.GetBytes(charArr);

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                if (!Stdout)
                {
                    using StreamWriter streamWriter = new StreamWriter(fileName);
                    streamWriter.Write(charArr, 0, charArr.Length);
                    streamWriter.Flush();
                }
                else
                {
                    using Stream stream = new MemoryStream(charBytes);
                    using StreamReader streamReader = new StreamReader(stream);

                    CsvDataReader csvcra = CsvDataReader.Create(streamReader);
                    using var csvdatawriter = CsvDataWriter.Create(fileName, Options);
                    csvdatawriter.Write(csvcra);
                }
            }

            return charBytes;
        }

        public byte[] WriteByAttribute<T>(List<T> listData, string fileName = "") where T : class
        {
            if (listData == null)
            {
                throw new AggregateException("Parameter cannot be null");
            }

            Dictionary<string, string> column = GetHeader<T>();
            return Write(listData, column, fileName);
        }

        public async Task<byte[]> WriteAsync<T>(List<T> listData, Dictionary<string, string> column, string fileName = "")
        {
            var bytearr = await Task.Run(() =>
            {
                return Write<T>(listData, column, fileName);
            });
            return bytearr;
        }

        public async Task<byte[]> WriteByAttributeAsync<T>(List<T> listData, string fileName = "") where T : class
        {
            var bytearr = await Task.Run(() =>
            {
                return WriteByAttribute<T>(listData, fileName);
            });
            return bytearr;
        }

        public StringBuilder BuildStringBuilder<T>(List<T> list, Dictionary<string, string> column)
        {
            List<object> strch = new List<object>();
            StringBuilder builder = new StringBuilder();
            var tab = "";
            if (!Stdout) tab = "	";
            
            var columns = column.Keys.Select(s => "\"" + s + tab + "\"");
            builder.AppendJoin<string>(',', columns);
            builder.Append(Environment.NewLine);
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

                    if (TimeFormatting != null && fieldvalue != null && fieldvalue is DateTime date)
                        fieldvalue = date.ToString(TimeFormatting);

                    if (ForMat != null)
                        fieldvalue = ForMat(i.Key, i.Value, fieldvalue);

                    if (fieldvalue == null) strch.Add("");
                    else strch.Add("\"" + fieldvalue.ToString() + tab + "\"");
                }

                builder.AppendJoin<object>(',', strch);
                builder.Append(Environment.NewLine);
                strch.Clear();
            }

            return builder;
        }

        public Dictionary<string, string> GetHeader<T>() where T : class
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
