using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace SampleWebapi
{
    public class ExportEntity
    {
        [Qin.CsvRelevant.ExportColumn("Myorderid")]
        public int Orderid { get; set; }

        [Qin.CsvRelevant.ExportColumn("myname")]
        public string Name { get; set; }

        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Area { get; set; }
        public int State { get; set; }
        public byte Isdel { get; set; }
        public decimal Money { get; set; }
    }
}
