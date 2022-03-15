using Sylvan.Data.Csv;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qin.CsvRelevant
{
    public interface ICsvGenerate
    {
        /// <summary>
        /// Time field format, default "yyyy MM DD HH: mm: SS"
        /// </summary>
        string TimeFormatting { get; set; }

        /// <summary>
        /// Combine CSV text content for formatting
        /// </summary>
        Func<string, string, object, object> ForMat { get; set; }

        CsvDataWriterOptions Options { get; set; }

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

        Dictionary<string, string> GetHeader<T>() where T : class;
    }
}
