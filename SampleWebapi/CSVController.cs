using Dapper;
using Microsoft.AspNetCore.Mvc;
using Qin.CsvRelevant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
                listData2.Add(new ExportEntity { Name = "wa" + i, Orderid = 66 + i, CreateTime = DateTime.Now, OtherTime = DateTime.Now });
            return listData2;
        }

        /// <summary>
        /// http://localhost:5000/CSV/Export0 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Export0()
        {
            var exece = CsvGenerateBuilder.Build();
            var exece3 = CsvGenerateBuilder.Build();
            bool flag = object.ReferenceEquals(exece, exece3);

            Console.WriteLine("Export0");
            /*
               List<dynamic> listData = new List<dynamic>();
               listData.Add(new { name = "Sully1,667\t", Poe = 12333, kkId = "46100152200609203123332", date = DateTime.Now });
               listData.Add(new { name = "Ben1,vv\t", Poe = 12333, kkId = "4610015220060920332332", date = DateTime.Now });
               listData.Add(new { name = "Fiy1,32,fds,we", Poe = 12333, kkId = "46100132522006092033", date = DateTime.Now });
               listData.Add(new { name = "Fiy1;", Poe = 12333, kkId = "4610015220060929873", date = DateTime.Now });
               listData.Add(new { name = "Fiy1|", Poe = 12333, kkId = "46100152200,609293321", date = DateTime.Now });
               listData.Add(new { name = "Fiy1", Poe = 12333, kkId = "4610015220060929233221", date = DateTime.Now });
               listData.Add(new { name = "Fiy1", Poe = 12334, kkId = "", date = DateTime.Now });
               listData.Add(new { name = "Fiy1", Poe = 12323, kkId = "", date = DateTime.Now });
               listData.Add(new { name = "Fiy1", Poe = 12313, kkId = "", date = DateTime.Now });
               Dictionary<string, string> column = new Dictionary<string, string>();
               column.Add("MyName,32", "name");
               column.Add("MyOrderId,rr", "Poe");
               column.Add("kk,Id", "kkId");
               column.Add("da,te", "date");
            */

            List<dynamic> listData = new List<dynamic>();
            listData.Add(new { name = "Sull,y1", Poe = "46100152200609203123332", kkId = "", date = DateTime.Now });
            listData.Add(new { name = "Ben1", Poe = "46100152200609203123332", kkId = "", date = DateTime.Now });

            Dictionary<string, string> column = new Dictionary<string, string>();
            column.Add("MyName", "name");
            column.Add("MyOrderId", "Poe");

            var path = Path.Combine(localexportpath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            _csvGenerate.Stdout = true; // Standardized output error, If the number of digits exceeds 15, truncate and supplement 0 in WPS
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

            StringBuilder stringBuilder = _csvGenerate.GetContent(listData2, culumn2);

            _csvGenerate.TimeFormatting = null;
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

            var charBytes0 = await _csvGenerate.WriteByAttributeAsync(listData, $"{localexportpath}\\Export2.csv");
            return File(charBytes0, "text/csv", "Export2a.csv");
        }

        /// <summary>
        /// WriteByAttributeAsync RemoveHead
        /// http://localhost:5000/CSV/Export3
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Export3()
        {
            Console.WriteLine("Export3");

            List<ExportEntity> listData = new List<ExportEntity>();
            listData.Add(new ExportEntity { Name = "Sully", Orderid = 12333, State = 1 });
            listData.Add(new ExportEntity { Name = "Ben", Orderid = 12333, State = 2 });
            listData.Add(new ExportEntity { Name = "Fiy", Orderid = 12333, State = 3 });

            string localexportpath = "Export";
            var path = Path.Combine(localexportpath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var head = _csvGenerate.GetHeader<ExportEntity>();
            var ppo = _csvGenerate.GetContent<ExportEntity>(listData, head);
            StringBuilder stringbuilder = new StringBuilder();
            {
                // The simplest removal method is to remove the head
                string p1 = "\"Myorderid  \",\"myname   \",\"myState  \"";
                ppo.Remove(0, p1.Length - 1 - 1);
            }
            {
                // There is a problem - report an error
                char[] oo = new char[ppo.Length];
                ppo.CopyTo(0, oo, 35, ppo.Length);
                var ss1 = new String(oo);
            }
            {
                // Feasible, but not optimal
                var ii1 = ppo.ToString().Split("\n");
                for (int i = 1; i < ii1.Length; i++)
                {
                    stringbuilder.Append(ii1[i]);
                }
            }

            char[] charArr = new char[stringbuilder.Length];
            stringbuilder.CopyTo(0, charArr, 0, stringbuilder.Length);
            byte[] charBytes = Encoding.Default.GetBytes(charArr);
            stringbuilder.Clear();

            return File(charBytes, "text/csv", "Export2cccca.csv");
        }

        /// <summary>
        /// WriteByAttributeAsync
        /// http://localhost:5000/CSV/Export4
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Export4()
        {
            var name = nameof(Export4);
            Console.WriteLine(name);

            List<ExportEntity> listData = new List<ExportEntity>();
            listData.Add(new ExportEntity { Name = "Sully", Orderid = 12333, State = 1 });
            listData.Add(new ExportEntity { Name = "Ben", Orderid = 12333, State = 2 });
            listData.Add(new ExportEntity { Name = "Fiy", Orderid = 12333, State = 3 });

            string localexportpath = "Export";
            var path = Path.Combine(localexportpath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            await _csvGenerate.WriteByAttributeAsync(listData, $"{localexportpath}\\{name}.csv");
            _csvGenerate.Append = true;
            _csvGenerate.RemoveHead = true;

            listData.Clear();
            listData.Add(new ExportEntity { Name = "Sully2", Orderid = 12333, State = 1 });
            listData.Add(new ExportEntity { Name = "Ben2", Orderid = 12333, State = 2 });
            listData.Add(new ExportEntity { Name = "Fiy2", Orderid = 12333, State = 3 });
          await _csvGenerate.WriteByAttributeAsync(listData, $"{localexportpath}\\{name}.csv");

            listData.Clear();
            listData.Add(new ExportEntity { Name = "Sully3", Orderid = 12333, State = 1 });
            listData.Add(new ExportEntity { Name = "Ben3", Orderid = 12333, State = 2 });
            listData.Add(new ExportEntity { Name = "Fiy3", Orderid = 12333, State = 3 });
          await _csvGenerate.WriteByAttributeAsync(listData, $"{localexportpath}\\{name}.csv");
            return Ok($"{localexportpath}\\{name}.csv, success");
        }

        /// <summary>
        /// WriteByAttributeAsync
        /// http://localhost:5000/CSV/Export5
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Export5()
        {
            var name = nameof(Export5);
            Console.WriteLine(name);

            List<ExportEntity> listData = new List<ExportEntity>();
            listData.Add(new ExportEntity { Name = "bob", Orderid = 12333, State = 1 });
            listData.Add(new ExportEntity { Name = "sos", Orderid = 12333, State = 2 });
            listData.Add(new ExportEntity { Name = "fif", Orderid = 12333, State = 3 });

            string localexportpath = "Export";
            var path = Path.Combine(localexportpath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

             _csvGenerate.RemoveHead = true;
            _csvGenerate.Append = false;
            await _csvGenerate.WriteByAttributeAsync(listData, $"{localexportpath}\\{name}.csv");
            return Ok($"{localexportpath}\\{name}.csv, success");
        }

        /// <summary>
        /// WriteByAttributeAsync
        /// http://localhost:5000/CSV/Export6
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Export6()
        {
            var name = nameof(Export6);
            Console.WriteLine(name);

            List<ExportEntity> listData = new List<ExportEntity>();
            listData.Add(new ExportEntity { Name = "bob", Orderid = 12333, State = 1 });

            string localexportpath = "Export";
            var path = Path.Combine(localexportpath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var connstr = await System.IO.File.ReadAllTextAsync("D://connstr.txt");
            var sqlstr = await System.IO.File.ReadAllTextAsync("D://sqlstr.txt");
            using var connStr = new MySqlConnector.MySqlConnection(connstr);
            connStr.Open();
            var reader = connStr.ExecuteReader(sqlstr);
            var func = reader.GetRowParser<ExportEntity>();
            await _csvGenerate.WritePhysicalFile($"{localexportpath}\\{name}.csv", reader, (reader) =>
            {
                var model = func(reader);
                return model;
            });
            return Ok($"{path} build success");
        }        
    }
}
