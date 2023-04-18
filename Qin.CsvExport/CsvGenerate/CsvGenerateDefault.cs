namespace Qin.CsvRelevant
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    internal class CsvGenerateDefault : ICsvGenerate
    {
        public string TimeFormatting { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public Func<string, string, object, object> ForMat { get; set; }
        public bool Stdout { get; set; } = false;
        public bool Append { get; set; } = false;
        public bool RemoveHead { get; set; } = false;

        public StringBuilder GetContent<T>(List<T> listData, Dictionary<string, string> column) => BuildStringBuilder(listData, column);

        public async Task WritePhysicalFile<T>(string path, IDataReader reader, Func<IDataReader, T> func)
            where T : class
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.WriteLine($"{path}; Exists, just deleted");
            }

            Dictionary<string, string> column = GetHeader<T>();
            StringBuilder stringbuilder = BuildStringBuilder(new List<T>(), column);
            path += ".temp";
            using StreamWriter streamWriter = new StreamWriter(path, append: false);
            int hadstringbuilderLength = stringbuilder.Length;
            char[] charHeadArr = new char[stringbuilder.Length];
            stringbuilder.CopyTo(0, charHeadArr, 0, stringbuilder.Length);
            await streamWriter.WriteAsync(charHeadArr, 0, charHeadArr.Length); // Write Head
            stringbuilder.Clear();

            streamWriter.Flush();
            streamWriter.Close();

            while (reader.Read())
            {
                T model = func(reader);

                stringbuilder = BuildStringBuilder(new List<T>() { model }, column);
                char[] charArr = new char[stringbuilder.Length - hadstringbuilderLength];
                stringbuilder.CopyTo(hadstringbuilderLength, charArr, 0, stringbuilder.Length - hadstringbuilderLength);

                using StreamWriter streamWriter2 = new StreamWriter(path, append: true);
                await streamWriter2.WriteAsync(charArr, 0, charArr.Length);

                stringbuilder.Clear();
                streamWriter2.Flush();
                streamWriter2.Close();
            }

            if (File.Exists(path))
            {
                var newPath = path.Replace(".temp", "");
                File.Move(path, newPath);
                Debug.WriteLine($"{newPath}; File generated successfully");
            }
        }

        public byte[] Write<T>(List<T> listData, Dictionary<string, string> column, string fileName = "")
        {
            if (listData == null || column == null)
            {
                throw new AggregateException("Parameter cannot be null");
            }
            listData.Capacity = listData.Count;

            StringBuilder stringbuilder = BuildStringBuilder(listData, column);
            char[] charArr = default;
            if (RemoveHead)
            {
                var hadstringbuilderLength = BuildStringBuilder(new List<T>(), column).Length;
                charArr = new char[stringbuilder.Length - hadstringbuilderLength];
                stringbuilder.CopyTo(hadstringbuilderLength, charArr, 0, stringbuilder.Length - hadstringbuilderLength);
            }
            else
            {
                charArr = new char[stringbuilder.Length];
                stringbuilder.CopyTo(0, charArr, 0, stringbuilder.Length);
            }
            byte[] charBytes = Encoding.Default.GetBytes(charArr);
            stringbuilder.Clear();

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileName, append: Append))
                {
                    streamWriter.Write(charArr, 0, charArr.Length);
                    streamWriter.Flush();
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

#if NET461 ||NETSTANDARD2_0_OR_GREATER
            builder.Append(string.Join(",", columns));
#else
            builder.AppendJoin<string>(',', columns);
#endif
            builder.Append(Environment.NewLine);
            Type type = null;
            PropertyInfo prop = null;
            object fieldvalue = null;
            if (list.Count > 0) type = list[0].GetType();

            foreach (var item in list)
            {
                foreach (var i in column)
                {
                    prop = type.GetProperty(i.Value);
                    if (prop == null)
                        throw new Exception($"There is no {i.Value} attribute in the {type.Name} class. Please check whether the attribute is written incorrectly");

                    fieldvalue = prop.GetValue(item);

                    if (TimeFormatting != null && fieldvalue != null && fieldvalue is DateTime date)
                        fieldvalue = date.ToString(TimeFormatting);

                    if (ForMat != null)
                        fieldvalue = ForMat(i.Key, i.Value, fieldvalue);

                    if (fieldvalue == null) strch.Add("");
                    else strch.Add("\"" + fieldvalue.ToString() + tab + "\"");
                }

#if NET461 ||NETSTANDARD2_0_OR_GREATER
                builder.Append(string.Join(",", strch));
#else
                builder.AppendJoin<object>(',', strch);
#endif
                builder.Append(Environment.NewLine);
                strch.Clear();
            }
            builder.Capacity = builder.Length;
            return builder;
        }

        public Dictionary<string, string> GetHeader<T>() where T : class
        {
            Type type = typeof(T);
            Type column = typeof(ExportColumnAttribute);
            ExportColumnAttribute portAttr = null;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (PropertyInfo item in type.GetProperties())
            {
                portAttr = item.GetCustomAttributes(column, true).Where(it => it is ExportColumnAttribute).SingleOrDefault() as ExportColumnAttribute;
                if (portAttr != null)
                    dic.Add(portAttr.ExcelColumnName, item.Name);
            }
            return dic;
        }
    }
}
