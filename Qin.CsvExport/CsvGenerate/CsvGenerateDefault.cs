namespace Qin.CsvRelevant
{
    using Microsoft.Extensions.ObjectPool;
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    internal class CsvGenerateDefault : ICsvGenerate
    {
        private static readonly ArrayPool<char> CharArrayPool = ArrayPool<char>.Shared;
        private static readonly ObjectPool<StringBuilder> StringBuilderPool = new DefaultObjectPoolProvider().CreateStringBuilderPool();

        public string TimeFormatting { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public Func<string, string, object, object> ForMat { get; set; }
        public bool Stdout { get; set; } = false;
        public bool Append { get; set; } = false;
        public bool RemoveHead { get; set; } = false;

        public StringBuilder GetContent<T>(List<T> listData, ReadOnlyDictionary<string, string> column)
        {
            var span_list = listData.ToArray().AsSpan();
            return BuildStringBuilder(span_list, column);
        }

        public async Task WritePhysicalFile<T>(string path, IDataReader reader, Func<IDataReader, T> func)
            where T : class
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.WriteLine($"{path}; Exists, just deleted");
            }

            ReadOnlyDictionary<string, string> column = GetHeader<T>();
            Span<T> span_list = Enumerable.Empty<T>().ToArray().AsSpan();
            StringBuilder stringbuilder = BuildStringBuilder(span_list, column);
            path += ".temp";
            using StreamWriter streamWriter = new StreamWriter(path, append: false);
            int hadstringbuilderLength = stringbuilder.Length;
            char[] charHeadArr = CharArrayPool.Rent(stringbuilder.Length);
            stringbuilder.CopyTo(0, charHeadArr, 0, stringbuilder.Length);
            await streamWriter.WriteAsync(charHeadArr, 0, stringbuilder.Length); // Write Head
            stringbuilder.Clear();
            CharArrayPool.Return(charHeadArr);

            streamWriter.Flush();
            streamWriter.Close();

            while (reader.Read())
            {
                T model = func(reader);

                Span<T> span_list2 = Enumerable.Empty<T>().ToArray().AsSpan();
                stringbuilder = BuildStringBuilder(span_list2, column);
                char[] charArr = CharArrayPool.Rent(stringbuilder.Length - hadstringbuilderLength);
                stringbuilder.CopyTo(hadstringbuilderLength, charArr, 0, stringbuilder.Length - hadstringbuilderLength);

                using StreamWriter streamWriter2 = new StreamWriter(path, append: true);
                await streamWriter2.WriteAsync(charArr, 0, stringbuilder.Length - hadstringbuilderLength);

                stringbuilder.Clear();
                CharArrayPool.Return(charArr);
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

        public byte[] Write<T>(List<T> listData, ReadOnlyDictionary<string, string> column, string fileName = "")
        {
            if (listData == null || column == null)
            {
                throw new AggregateException("Parameter cannot be null");
            }
            listData.Capacity = listData.Count;
            Span<T> span_list = listData.ToArray().AsSpan();
            StringBuilder stringbuilder = BuildStringBuilder(span_list, column);
            char[] charArr = default;
            if (RemoveHead)
            {
                Span<T> empty_span_list = Enumerable.Empty<T>().ToArray().AsSpan();
                var hadstringbuilderLength = BuildStringBuilder(empty_span_list, column).Length;
                charArr = CharArrayPool.Rent(stringbuilder.Length - hadstringbuilderLength);
                stringbuilder.CopyTo(hadstringbuilderLength, charArr, 0, stringbuilder.Length - hadstringbuilderLength);
            }
            else
            {
                charArr = CharArrayPool.Rent(stringbuilder.Length);
                stringbuilder.CopyTo(0, charArr, 0, stringbuilder.Length);
            }
            byte[] charBytes = Encoding.Default.GetBytes(charArr, 0, stringbuilder.Length);
            stringbuilder.Clear();
            CharArrayPool.Return(charArr);

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileName, append: Append))
                {
                    streamWriter.Write(charArr, 0, stringbuilder.Length);
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

            ReadOnlyDictionary<string, string> column = GetHeader<T>();
            return Write(listData, column, fileName);
        }

        public async Task<byte[]> WriteAsync<T>(List<T> listData, ReadOnlyDictionary<string, string> column, string fileName = "")
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

        public StringBuilder BuildStringBuilder<T>(Span<T> list, ReadOnlyDictionary<string, string> column)
        {
            List<object> strch = new List<object>();
            StringBuilder builder = StringBuilderPool.Get();
            var tab = "";
            if (!Stdout) tab = "	";
            var columns = column.Keys.Select(s => "\"" + s + tab + "\"");

#if NET461 || NETSTANDARD2_0_OR_GREATER
                builder.Append(string.Join(",", columns));
#else
            builder.AppendJoin<string>(',', columns);
#endif
            builder.Append(Environment.NewLine);
            Type type = null;
            PropertyInfo prop = null;
            object fieldvalue = null;
            // if (list.Count > 0) type = list[0].GetType();
            type = list[0]?.GetType();
            if (type == null)
            {
                return builder;
            }

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

#if NET461 || NETSTANDARD2_0_OR_GREATER
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

        public ReadOnlyDictionary<string, string> GetHeader<T>() where T : class
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
            return new ReadOnlyDictionary<string, string>(dic);
        }
    }
}
