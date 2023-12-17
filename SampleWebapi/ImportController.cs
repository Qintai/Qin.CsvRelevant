using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Qin.CsvRelevant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SampleWebapi
{
    public class ImportController : Controller
    {
        ICsvGenerate _csvGenerate;
        string localexportpath = "Export";
        ILogger<ImportController> _logger;

        public ImportController(ICsvGenerate csv, ILogger<ImportController> logger)
        {
            _csvGenerate = csv;
            _logger = logger;
        }

        class Tmp1 { public string name { get; set; } public string cj { get; set; } }

        /// <summary>
        /// import
        /// http://localhost:5000/Import/Import0
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Import0()
        {
            var name = nameof(Import0);
            Console.WriteLine(name);

            const string V = @"D:\Test_CsvRelevant.csv";
            Stream ss = new FileStream(V, FileMode.OpenOrCreate);
            var dd =  await _csvGenerate.Import<ImportController>(ss);
            foreach (Dictionary<string, string> item in dd)
            {
                var model1 = JsonConvert.DeserializeObject<Tmp1>(JsonConvert.SerializeObject(item));
            }

            return Ok($"Import0");
        }
    }
}
