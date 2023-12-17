namespace Qin.CsvRelevant
{
    using System;
    using System.Data;
    using System.Threading.Tasks;

    public partial interface ICsvGenerate
    {
        /// <summary>
        /// 写入物理文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">需要写入的文件路径</param>
        /// <param name="reader">数据源</param>
        /// <param name="func">读取数据后的处理</param>
        /// <returns></returns>
        Task WritePhysicalFile<T>(string path, IDataReader reader, Func<IDataReader, T> func) where T : class;

        /// <summary>
        /// 写入物理文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">需要写入的文件路径</param>
        /// <param name="config">关于db的配置</param>
        /// <param name="action">读取数据后的处理</param>
        /// <returns></returns>
        Task WritePhysicalFile<T>(string path, WritePhysicalFileDbConfig config, Action<T>? action = null) where T : class;
    }
}
