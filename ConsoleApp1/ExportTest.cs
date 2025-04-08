using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Qin.CsvRelevant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    [MemoryDiagnoser(true)]
    [Config(typeof(Config))]  // 添加这行
    public class ExportTest
    {
       // 添加配置类
        private class Config : ManualConfig
        {
            public Config()
            {
                WithOptions(ConfigOptions.DisableOptimizationsValidator);
            }
        }

        [Benchmark]
        public void Export()
        {
            var csvGenerate = CsvGenerateBuilder.Build();
            csvGenerate.Stdout = false;

            List<dynamic> listData = new List<dynamic>();
            listData.Add(new { name = "Sull,y1", Poe = "46100152200609203123332", kkId = "", date = DateTime.Now });
            listData.Add(new { name = "Ben1", Poe = "46100152200609203123332", kkId = "", date = DateTime.Now });

            Dictionary<string, string> column = new Dictionary<string, string>();
            column.Add("MyName", "name");
            column.Add("MyOrderId", "Poe");

            string localexportpath = "Export";
            var path = Path.Combine(localexportpath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var charBytes0 = csvGenerate.Write(listData, column, Path.Combine(localexportpath, "ConsoleApp1.csv"));
        }

        [Benchmark]
        public async Task Export4()
        {
            var csvGenerate = CsvGenerateBuilder.Build();
            var name = nameof(Export4);
            List<ExportEntity> listData = new List<ExportEntity>();
            listData.Add(new ExportEntity { Name = "Sully", Orderid = 12333, State = 1 });
            listData.Add(new ExportEntity { Name = "Ben", Orderid = 12333, State = 2 });
            listData.Add(new ExportEntity { Name = "Fiy", Orderid = 12333, State = 3 });

            string localexportpath = "Export";
            var path = Path.Combine(localexportpath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            await csvGenerate.WriteByAttributeAsync(listData, $"{localexportpath}\\{name}.csv");
            // csvGenerate.Append = true;
            // csvGenerate.RemoveHead = true;
            listData.Clear();
            listData.Add(new ExportEntity { Name = "Sully2", Orderid = 12333, State = 1 });
            listData.Add(new ExportEntity { Name = "Ben2", Orderid = 12333, State = 2 });
            listData.Add(new ExportEntity { Name = "Fiy2", Orderid = 12333, State = 3 });
            await csvGenerate.WriteByAttributeAsync(listData, $"{localexportpath}\\{name}.csv");

            listData.Clear();
            listData.Add(new ExportEntity { Name = "Sully3", Orderid = 12333, State = 1 });
            listData.Add(new ExportEntity { Name = "Ben3", Orderid = 12333, State = 2 });
            listData.Add(new ExportEntity { Name = "Fiy3", Orderid = 12333, State = 3 });
            await csvGenerate.WriteByAttributeAsync(listData, $"{localexportpath}\\{name}.csv");
            string pp = $"{localexportpath}\\{name}.csv";
        }
    }
}
