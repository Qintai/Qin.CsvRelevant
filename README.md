# Qin.CsvRelevant
Csv Export    Install

```xml
   Install-Package Qin.CsvRelevant -Version 1.0.3.1
```



``` C#
public void ConfigureServices(IServiceCollection services)
{
  services.AddCsvGenerate();
}
```

## 1.Customize CSV file header
``` C#
  List<dynamic> listData = new List<dynamic>();
  listData.Add(new { name = "Sully1", Poe = 12333 });
  listData.Add(new { name = "Ben1", Poe = 12333 });
  listData.Add(new { name = "Fiy1", Poe = 12333 });
  Dictionary<string, string> column = new Dictionary<string, string>();
  column.Add("MyName", "name");
  column.Add("MyOrderId", "Poe");
  var charBytes = await _csvGenerate.WriteAsync(listData, column, "xxx\\demo1.csv");
```

## 2.Specify the mapping relationship through fluent
``` C#
 List<ExportEntity> listData2 = GetList();
 var culumn2 = _csvGenerate.Map<ExportEntity>("MyOrderId", a => a.Orderid)
                                      .Map<ExportEntity>("MyName", a => a.Name)
                                      .Map<ExportEntity>("MyName2", a => a.Name2)
                                      .Map<ExportEntity>("MyName3", a => a.Name3)
                                      .Map<ExportEntity>("State", a => a.State)
                                      .Map<ExportEntity>("Money", a => a.Money)
                                      .Map<ExportEntity>("Isdel", a => a.Isdel)
                                      .Map<ExportEntity>("Area", a => a.Area).BuildDictionary();

 _csvGenerate.ForMat = (column, fieldname, fieldvalue) =>
 {
     if (fieldvalue is null)
         return fieldvalue;

     if (column == "Date" && fieldname == "CreateTime")
     {
         fieldvalue = fieldvalue is DateTime s1 ? s1.ToString("yyyy-MM-dd") : fieldvalue;
     }
     return fieldvalue;
 };

 var charBytes2 = await _csvGenerate.WriteAsync(listData2, culumn2, "xxx\\demo2.csv");
```

## 3.Entity tag mapping column name
``` C#
    public class ExportEntity
    {
        [Qin.CsvRelevant.ExportColumn("Myorderid")]
        public int Orderid { get; set; }

        [Qin.CsvRelevant.ExportColumn("myname")]
        public string Name { get; set; }
        
        [Qin.CsvRelevant.ExportColumn("Date")]
        public DateTime CreateTime { get; set; }
    }
    // Split line
    List<ExportEntity> listData = new List<ExportEntity>();
    listData.Add(new ExportEntity { Name = "Sully", Orderid = 12333 });
    listData.Add(new ExportEntity { Name = "Ben", Orderid = 12333 });
    listData.Add(new ExportEntity { Name = "Fiy", Orderid = 12333 });

    var charBytes0 = await _csvGenerate.WriteByAttributeAsync(listData, "xxx\\demo3.csv");
```

When writing a CSV file, when the "file name" field is null or the default value, byte [] will be returned, but the file will not be generated



