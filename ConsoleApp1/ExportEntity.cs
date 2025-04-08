using System;

namespace ConsoleApp1
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

        [Qin.CsvRelevant.ExportColumn("myState")]
        public int State { get; set; }
        public byte Isdel { get; set; }
        public decimal Money { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? OtherTime { get; set; }
    }
}
