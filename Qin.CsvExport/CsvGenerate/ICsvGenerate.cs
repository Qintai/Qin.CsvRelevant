using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qin.CsvRelevant
{
    public interface ICsvGenerate
    {
        bool FormatTextOutput { get; set; }
        Func<string, string, object, object> ForMat { get; set; }
        string TimeFormatting { get; set; }

        byte[] Write<T>(List<T> listData, Dictionary<string, string> column, string fileName = "", Func<string, object, object> propOperation = null);

        Task<byte[]> WriteAsync<T>(List<T> listData, Dictionary<string, string> column, string fileName = "", Func<string, object, object> propOperation = null);

        byte[] WriteByAttribute<T>(List<T> listData, string fileName = "", Func<string, object, object> propOperation = null) where T : class;

        Task<byte[]> WriteByAttributeAsync<T>(List<T> listData, string fileName = "", Func<string, object, object> propOperation = null) where T : class;

        Dictionary<string, string> GetHeader<T>() where T : class;
    }
}
