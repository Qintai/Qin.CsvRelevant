namespace Qin.CsvRelevant
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Threading.Tasks;

    public partial interface ICsvGenerate
    {
        /// <summary>
        /// 导入Csv
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Task<List<Dictionary<string, string>>> Import<T>(Stream stream) where T : class;
    }
}
