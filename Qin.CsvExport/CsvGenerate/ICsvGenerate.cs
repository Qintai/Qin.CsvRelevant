using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qin.CsvRelevant
{
    public interface ICsvGenerate
    {
        byte[] Write<T>(List<T> listData, Dictionary<string, string> column, string fileName = "");

        Task<byte[]> WriteAsync<T>(List<T> listData, Dictionary<string, string> column, string fileName = "");

        byte[] WriteByAttribute<T>(List<T> listData, string fileName = "");

        Task<byte[]> WriteByAttributeAsync<T>(List<T> listData, string fileName = "");
    }
}
