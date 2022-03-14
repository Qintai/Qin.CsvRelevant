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
        string localexportpath = "Export";

        public CSVController(ICsvGenerate csv)
        {
            _csvGenerate = csv;
        }

        private List<ExportEntity> GetList()
        {
            List<ExportEntity> listData2 = new List<ExportEntity>();
            for (int i = 0; i < 40000; i++)
                listData2.Add(new ExportEntity { Name = "wa" + i, Orderid = 66 + i, CreateTime = DateTime.Now ,OtherTime =DateTime.Now});
            return listData2;
        }

        /// <summary>
        /// http://localhost:5000/CSV/Export0
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Export0()
        {
            Console.WriteLine("Export0");
            List<dynamic> listData = new List<dynamic>();
            listData.Add(new { name = "Sully1", Poe = 12333, kkId = "461001522006092031", date = DateTime.Now });
            listData.Add(new { name = "Ben1", Poe = 12333,   kkId = "461001522006092032", date = DateTime.Now });
            listData.Add(new { name = "Fiy1", Poe = 12333,   kkId = "461001522006092033", date = DateTime.Now });
            listData.Add(new { name = "Fiy1", Poe = 12333,   kkId = "461001522006092987", date = DateTime.Now });
            listData.Add(new { name = "Fiy1", Poe = 12333,   kkId = "461001522006092921", date = DateTime.Now });
            listData.Add(new { name = "Fiy1", Poe = 12333,   kkId = "461001522006092921", date = DateTime.Now });
            listData.Add(new { name = "Fiy1", Poe = 12334,   kkId = "", date = DateTime.Now });
            listData.Add(new { name = "Fiy1", Poe = 12323,   kkId = "", date = DateTime.Now });
            listData.Add(new { name = "Fiy1", Poe = 12313,   kkId = "", date = DateTime.Now });
            Dictionary<string, string> column = new Dictionary<string, string>();
            column.Add("MyName", "name");
            column.Add("MyOrderId", "Poe");
            column.Add("kkId", "kkId");
            column.Add("date", "date");

            var path = Path.Combine(localexportpath);
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            _csvGenerate.FormatTextOutput = true;

            var charBytes0 = await _csvGenerate.WriteAsync(listData, column, Path.Combine(localexportpath, "export0.csv"));
            return Ok(0); 
        }

        /// <summary>
        /// WriteAsync£¬Map£¬ZipFileByGb2312
        /// http://localhost:5000/CSV/Export1
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Export1()
        {
            //Specify the mapping relationship through fluent
            List<ExportEntity> listData2 = GetList();
            var culumn2 = _csvGenerate.Map<ExportEntity>("MyOrderId", a => a.Orderid)
                                      //.Map<ExportEntity>("MyName", a => a.Name)
                                      //.Map<ExportEntity>("MyName2", a => a.Name)
                                      //.Map<ExportEntity>("MyName3", a => a.Name3)
                                      //.Map<ExportEntity>("State", a => a.State)
                                      //.Map<ExportEntity>("Money", a => a.Money)
                                      //.Map<ExportEntity>("Isdel", a => a.Isdel)
                                      .Map<ExportEntity>("Date", a => a.CreateTime)
                                      .Map<ExportEntity>("Time", a => a.CreateTime)
                                      .Map<ExportEntity>("Area", a => a.Area)
                                      .Map<ExportEntity>("OtherTime", a => a.OtherTime).BuildDictionary();

            _csvGenerate.ForMat = (column, fieldname, fieldvalue) =>
            {
                if (fieldvalue is null)
                    return fieldvalue;

                if (column == "Date" && fieldname == "CreateTime")
                {
                    fieldvalue = fieldvalue is DateTime s1 ? s1.ToString("yyyy-MM-dd") : fieldvalue;
                }
                else if (column == "Time" && fieldname == "CreateTime")
                {
                    fieldvalue = fieldvalue is DateTime s1 ? s1.ToString("HH:mm:ss") : fieldvalue;
                }
                return fieldvalue;
            };
            
            var charBytes2 = await _csvGenerate.WriteAsync(listData2, culumn2, Path.Combine(localexportpath, "export1.csv"));
            return Ok(1); //File is SampleWebapi\Export
        }

        /// <summary>
        /// WriteByAttributeAsync
        /// http://localhost:5000/CSV/Export2
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Export2()
        {
            Console.WriteLine("Export2");

            List<ExportEntity> listData = new List<ExportEntity>();
            listData.Add(new ExportEntity { Name = "Sully", Orderid = 12333, State = 1 });
            listData.Add(new ExportEntity { Name = "Ben", Orderid = 12333, State = 2 });
            listData.Add(new ExportEntity { Name = "Fiy", Orderid = 12333, State = 3 });

            string localexportpath = "Export";
            var path = Path.Combine(localexportpath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Func<string, object, object> func = (fieldname, fieldvalue) =>
            {
                if (fieldname == "State")
                {
                    if (fieldvalue is int a && a == 1) return "ok";
                    else if (fieldvalue is int b && b == 2) return "fail";
                    else if (fieldvalue is int v && v == 3) return "no";
                    return "";
                }
                return fieldvalue;
            };

            var charBytes0 = await _csvGenerate.WriteByAttributeAsync(listData, $"{localexportpath}\\Export2.csv", func);
            return File(charBytes0, "text/csv", "Export2a.csv");
        }
    }
}
