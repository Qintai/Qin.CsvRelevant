using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Qin.CsvRelevant;

namespace SampleWebapi
{
    public class CSVController : Controller
    {
        ICsvGenerate _csvGenerate;
        IZipOperation _zipOperation;

        public CSVController(ICsvGenerate csv, IZipOperation zip)
        {
            _csvGenerate = csv;
            _zipOperation = zip;
        }

        private List<ExportEntity> GetList()
        {
            List<ExportEntity> listData2 = new List<ExportEntity>();
            for (int i = 0; i < 400000; i++)
                listData2.Add(new ExportEntity { Name = "wa" + i, Orderid = 66 + i });
            return listData2;
        }

        /// <summary>
        /// WriteAsync£¬Map£¬ZipFileByGb2312
        /// http://localhost:5000/CSV/Export
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Export()
        {
            Console.WriteLine("Export");

            List<dynamic> listData = new List<dynamic>();
            listData.Add(new { name = "Sully1", Poe = 12333 });
            listData.Add(new { name = "Ben1", Poe = 12333 });
            listData.Add(new { name = "Fiy1", Poe = 12333 });
            Dictionary<string, string> column = new Dictionary<string, string>();
            column.Add("MyName", "name");
            column.Add("MyOrderId", "Poe");

            string localexportpath = "Export";
            var path = Path.Combine(localexportpath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var charBytes0 = await _csvGenerate.WriteAsync(listData, column, $"{localexportpath}\\qqq.csv");

            //Specify the mapping relationship through fluent
            List<ExportEntity> listData2 = GetList();
            var culumn2 = _csvGenerate.Map<ExportEntity>("MyOrderId", a => a.Orderid)
                                      .Map<ExportEntity>("MyName", a => a.Name)
                                      .Map<ExportEntity>("MyName2", a => a.Name2)
                                      .Map<ExportEntity>("MyName3", a => a.Name3)
                                      .Map<ExportEntity>("State", a => a.State)
                                      .Map<ExportEntity>("Money", a => a.Money)
                                      .Map<ExportEntity>("Isdel", a => a.Isdel)
                                      .Map<ExportEntity>("Area", a => a.Area).BuildDictionary();

            var charBytes2 = await _csvGenerate.WriteAsync(listData2, culumn2, $"{localexportpath}\\qqq2.csv");

            //Compressed file
            _ = _zipOperation.ZipFileByGb2312($"{localexportpath}\\qqq.csv", $"{localexportpath}\\qqq.zip", "1");
            return File(charBytes2, "text/csv", "results.csv");
        }

        /// <summary>
        /// WriteByAttributeAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Export2()
        {
            Console.WriteLine("Export2");

            List<ExportEntity> listData = new List<ExportEntity>();
            listData.Add(new ExportEntity { Name = "Sully", Orderid = 12333 });
            listData.Add(new ExportEntity { Name = "Ben", Orderid = 12333 });
            listData.Add(new ExportEntity { Name = "Fiy", Orderid = 12333 });

            string localexportpath = "Export";
            var path = Path.Combine(localexportpath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var charBytes0 = await _csvGenerate.WriteByAttributeAsync(listData, $"{localexportpath}\\qqq.csv");
            _ = _zipOperation.ZipFileByGb2312($"{localexportpath}\\qqq.csv", $"{localexportpath}\\qqq.zip", "1");
            return File(charBytes0, "text/csv", "results.csv");
        }
    }
}
