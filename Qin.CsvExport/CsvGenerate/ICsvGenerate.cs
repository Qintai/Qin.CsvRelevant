namespace Qin.CsvRelevant
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICsvGenerate
    {
        /// <summary>
        /// Time field format, default "yyyy-MM-dd HH:mm:ss"
        /// </summary>
        string TimeFormatting { get; set; }

        /// <summary>
        /// Combine CSV text content for formatting
        /// </summary>
        Func<string, string, object, object> ForMat { get; set; }

        /// <summary>
        /// Original Output, default false, Add tab by default \t
        /// </summary>
        bool Stdout { get; set; }

        /// <summary>
        /// Writing physical files does not occupy memory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="reader"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        Task WritePhysicalFile<T>(string path, IDataReader reader, Func<IDataReader, T> func) where T : class;

        /// <summary>
        ///  true to append data to the file; false to overwrite the file. If the specified  file does not exist, this parameter has no effect, and the constructor creates a new file.
        /// </summary>
        bool Append { get; set; }

        /// <summary>
        /// Delete header
        /// </summary>
        bool RemoveHead { get; set; }

        byte[] Write<T>(List<T> listData, ReadOnlyDictionary<string, string> column, string fileName = "");

        /// <summary>
        /// WriteCsv
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listData">data source</param>
        /// <param name="column">header</param>
        /// <param name="fileName">No file name, no file is written, but byte [] is returned</param>
        /// <returns></returns>
        Task<byte[]> WriteAsync<T>(List<T> listData, ReadOnlyDictionary<string, string> column, string fileName = "");

        byte[] WriteByAttribute<T>(List<T> listData, string fileName = "") where T : class;

        /// <summary>
        /// WriteByAttribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listData">data source</param>
        /// <param name="fileName">No file name, no file is written, but byte [] is returned</param>
        /// <returns></returns>
        Task<byte[]> WriteByAttributeAsync<T>(List<T> listData, string fileName = "") where T : class;

        /// <summary>
        /// Get first column
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ReadOnlyDictionary<string, string> GetHeader<T>() where T : class;

        /// <summary>
        /// Get first column
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listData"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        StringBuilder GetContent<T>(List<T> listData, ReadOnlyDictionary<string, string> column);
    }
}
