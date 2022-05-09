namespace Qin.CsvRelevant
{
    using System;
    using System.Collections.Generic;
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
        ///  true to append data to the file; false to overwrite the file. If the specified  file does not exist, this parameter has no effect, and the constructor creates a new file.
        /// </summary>
        bool Append { get; set; }

        /// <summary>
        /// Delete header
        /// </summary>
        bool RemoveHead { get; set; }

        byte[] Write<T>(List<T> listData, Dictionary<string, string> column, string fileName = "");

        /// <summary>
        /// WriteCsv
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listData">data source</param>
        /// <param name="column">header</param>
        /// <param name="fileName">No file name, no file is written, but byte [] is returned</param>
        /// <returns></returns>
        Task<byte[]> WriteAsync<T>(List<T> listData, Dictionary<string, string> column, string fileName = "");

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
        Dictionary<string, string> GetHeader<T>() where T : class;

        StringBuilder GetContent<T>(List<T> listData, Dictionary<string, string> column);
    }
}
