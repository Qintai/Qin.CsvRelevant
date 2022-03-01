using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qin.CsvRelevant
{
    public interface ICsvGenerate
    {
        byte[] Write<T>(List<T> listData, Dictionary<string, string> column, string fileName = "", Func<string, object, string> propOperation = null);

        Task<byte[]> WriteAsync<T>(List<T> listData, Dictionary<string, string> column, string fileName = "", Func<string, object, string> propOperation = null);

        byte[] WriteByAttribute<T>(List<T> listData, string fileName = "", Func<string, object, string> propOperation = null);

        Task<byte[]> WriteByAttributeAsync<T>(List<T> listData, string fileName = "", Func<string, object, string> propOperation = null);
    }
}
