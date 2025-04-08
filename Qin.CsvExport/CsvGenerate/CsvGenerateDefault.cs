namespace Qin.CsvRelevant
{
    using Microsoft.Extensions.ObjectPool;
    using System;
    using System.Buffers;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// CSV生成器的默认实现类
    /// </summary>
    internal class CsvGenerateDefault : ICsvGenerate
    {
        /// <summary>
        /// 字符数组池，用于复用字符数组以减少内存分配
        /// </summary>
        private static readonly ArrayPool<char> CharArrayPool = ArrayPool<char>.Shared;

        /// <summary>
        /// StringBuilder对象池，用于复用StringBuilder实例以提升性能
        /// </summary>
        private static readonly ObjectPool<StringBuilder> StringBuilderPool = new DefaultObjectPoolProvider().CreateStringBuilderPool();

        /// <summary>
        /// 属性信息缓存，用于存储类型的属性反射信息，避免重复反射
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

        /// <summary>
        /// 缓冲区大小：80KB，用于文件读写操作
        /// </summary>
        private const int BufferSize = 81920; // 80KB buffer

        /// <summary>
        /// 表头缓存，用于存储类型的CSV表头信息，避免重复解析特性
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ReadOnlyDictionary<string, string>> HeaderCache = new();

        /// <summary>
        /// 日期时间的格式化字符串
        /// </summary>
        public string TimeFormatting { get; set; } = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 自定义格式化委托，用于自定义字段的格式化逻辑
        /// 参数：列名、属性名、属性值
        /// 返回：格式化后的值
        /// </summary>
        public Func<string, string, object, object> ForMat { get; set; }

        /// <summary>
        /// 是否输出到标准输出流，影响制表符的处理
        /// </summary>
        public bool Stdout { get; set; } = false;

        /// <summary>
        /// 写入文件时是否采用追加模式
        /// </summary>
        public bool Append { get; set; } = false;

        /// <summary>
        /// 是否移除CSV的表头信息
        /// </summary>
        public bool RemoveHead { get; set; } = false;

        public StringBuilder GetContent<T>(List<T> listData, ReadOnlyDictionary<string, string> column)
        {
            var span_list = listData.ToArray().AsSpan();
            return BuildStringBuilder(span_list, column);
        }

        public async Task WritePhysicalFile<T>(string path, IDataReader reader, Func<IDataReader, T> func)
            where T : class
        {
            var tempPath = path + ".temp";
            var column = GetHeader<T>();

            using var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize);
            using var bufferedStream = new BufferedStream(fileStream, BufferSize);
            using var writer = new StreamWriter(bufferedStream, Encoding.UTF8);

            // 写表头
            var headerBuilder = BuildStringBuilder(Span<T>.Empty, column);
            await writer.WriteAsync(headerBuilder.ToString());
            StringBuilderPool.Return(headerBuilder);

            // 写表行数据
            var buffer = CharArrayPool.Rent(BufferSize);
            try
            {
                while (reader.Read())
                {
                    var model = func(reader);
                    var builder = BuildStringBuilder(new Span<T>(new[] { model }), column);
                    await writer.WriteAsync(builder.ToString());
                    StringBuilderPool.Return(builder);
                }
            }
            finally
            {
                CharArrayPool.Return(buffer);
            }

            await writer.FlushAsync();

            // 原子操作替换文件
            if (File.Exists(path))
            {
                File.Replace(tempPath, path, null);
            }
            else
            {
                File.Move(tempPath, path);
            }
        }

        public StringBuilder BuildStringBuilder<T>(Span<T> list, ReadOnlyDictionary<string, string> column)
        {
            var builder = StringBuilderPool.Get();
            builder.Clear();

            // 预分配容量
            var estimatedCapacity = (column.Count * 20 + 2) * (list.Length + 1);
            if (builder.Capacity < estimatedCapacity)
            {
                builder.Capacity = estimatedCapacity;
            }

            // 写入表头
            var tab = Stdout ? "" : "	";
            var isFirst = true;
            foreach (var key in column.Keys)
            {
                if (!isFirst) builder.Append(',');
                builder.Append('"').Append(key).Append(tab).Append('"');
                isFirst = false;
            }
            builder.AppendLine();

            if (list.IsEmpty || list[0] == null) return builder;

            // 缓存类型的属性信息
            var type = list[0].GetType();
            var properties = PropertyCache.GetOrAdd(type, t =>
                column.Values.Select(v => t.GetProperty(v)).ToArray());

            // 写入数据行
            foreach (var item in list)
            {
                isFirst = true;
                foreach (var prop in properties)
                {
                    if (!isFirst) builder.Append(',');
                    var value = prop.GetValue(item);

                    if (value == null)
                    {
                        builder.Append("\"\"");
                    }
                    else if (value is DateTime date)
                    {
                        builder.Append('"').Append(date.ToString(TimeFormatting)).Append(tab).Append('"');
                    }
                    else
                    {
                        builder.Append('"').Append(value.ToString()).Append(tab).Append('"');
                    }
                    isFirst = false;
                }
                builder.AppendLine();
            }

            return builder;
        }

        public ReadOnlyDictionary<string, string> GetHeader<T>() where T : class
        {
            return HeaderCache.GetOrAdd(typeof(T), type =>
            {
                var column = typeof(ExportColumnAttribute);
                var properties = type.GetProperties();
                var dictionary = new Dictionary<string, string>();

                foreach (var prop in properties)
                {
                    var attr = prop.GetCustomAttributes(column, true)
                        .OfType<ExportColumnAttribute>()
                        .FirstOrDefault();

                    if (attr != null)
                    {
                        dictionary.Add(attr.ExcelColumnName, prop.Name);
                    }
                }

                return new ReadOnlyDictionary<string, string>(dictionary);
            });
        }

        public async Task<byte[]> WriteAsync<T>(List<T> listData, ReadOnlyDictionary<string, string> column, string fileName = "")
        {
            if (listData == null)
                throw new ArgumentNullException(nameof(listData));
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            byte[] result;
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, BufferSize))
                {
                    var builder = BuildStringBuilder([.. listData], column);
                    try
                    {
                        if (RemoveHead)
                        {
                            var headerLength = BuildStringBuilder(Span<T>.Empty, column).Length;
                            await writer.WriteAsync(builder.ToString(headerLength, builder.Length - headerLength));
                        }
                        else
                        {
                            await writer.WriteAsync(builder.ToString());
                        }
                        await writer.FlushAsync();
                        result = memoryStream.ToArray();
                    }
                    finally
                    {
                        StringBuilderPool.Return(builder);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                using var fileStream = new FileStream(fileName, Append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None, BufferSize);
                using var bufferedStream = new BufferedStream(fileStream, BufferSize);
                await fileStream.WriteAsync(result.AsMemory(0, result.Length));
            }

            return result;
        }

        public Task<byte[]> WriteByAttributeAsync<T>(List<T> listData, string fileName = "") where T : class
        {
            if (listData == null)
                throw new ArgumentNullException(nameof(listData));

            var column = GetHeader<T>();
            return WriteAsync(listData, column, fileName);
        }

        public byte[] Write<T>(List<T> listData, ReadOnlyDictionary<string, string> column, string fileName = "")
        {
            if (listData == null)
                throw new ArgumentNullException(nameof(listData));
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8, BufferSize);
            
            var builder = BuildStringBuilder([.. listData], column);
            try
            {
                if (RemoveHead)
                {
                    var headerLength = BuildStringBuilder(Span<T>.Empty, column).Length;
                    writer.Write(builder.ToString(headerLength, builder.Length - headerLength));
                }
                else
                {
                    writer.Write(builder.ToString());
                }
                writer.Flush();
                var result = memoryStream.ToArray();

                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    using var fileStream = new FileStream(fileName, Append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None, BufferSize);
                    using var bufferedStream = new BufferedStream(fileStream, BufferSize);
                    fileStream.Write(result, 0, result.Length);
                }

                return result;
            }
            finally
            {
                StringBuilderPool.Return(builder);
            }
        }

        public byte[] WriteByAttribute<T>(List<T> listData, string fileName = "") where T : class
        {
            if (listData == null)
                throw new ArgumentNullException(nameof(listData));
            
            var column = GetHeader<T>();
            return Write(listData, column, fileName);
        }
    }
}