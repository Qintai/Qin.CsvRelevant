using Qin.CsvRelevant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
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
    }
}
