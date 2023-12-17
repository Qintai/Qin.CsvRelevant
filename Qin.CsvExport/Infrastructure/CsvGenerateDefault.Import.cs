namespace Qin.CsvRelevant
{
    using Dapper;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    internal partial class CsvGenerateDefault
    {
        public async Task<List<Dictionary<string, string>>> Import<T>(Stream stream) where T : class
        {
            List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string[] headers = (await reader.ReadLineAsync()).Split(',');
                while (!reader.EndOfStream)
                {
                    string[] values = (await reader.ReadLineAsync()).Split(',');
                    if (values.Length != headers.Length)
                    {
                        continue;
                    }
                    Dictionary<string, string> row = new Dictionary<string, string>();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        row[headers[i]] = values[i];
                    }
                    data.Add(row);
                }
                return data;
            }
        }

    }
}
