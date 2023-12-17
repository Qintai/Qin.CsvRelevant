namespace Qin.CsvRelevant
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public partial interface ICsvGenerate
    {
        /// <summary>
        /// 导出日期格式, 默认是 "yyyy-MM-dd HH:mm:ss"
        /// </summary>
        string TimeFormatting { get; set; }

        /// <summary>
        /// 组合CSV文本内容进行格式化
        /// </summary>
        Func<string, string, object, object>? ForMat { get; set; }

        /// <summary>
        /// 输出内容是否带\t，默认 true
        /// </summary>
        bool Stdout { get; set; }

        /// <summary>
        ///  true将数据附加到文件中；false覆盖文件。如果指定的文件不存在，则此参数无效，构造函数将创建一个新文件。
        /// </summary>
        bool Append { get; set; }

        /// <summary>
        /// 删除csv头
        /// </summary>
        bool RemoveHead { get; set; }

        /// <summary>
        /// 写入 byte
        /// </summary>
        byte[] Write<T>(List<T> listData, Dictionary<string, string> column, string fileName = "");

        /// <summary>
        /// 写入 byte
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listData">数据源</param>
        /// <param name="column">列</param>
        /// <param name="fileName">如果没有文件名，没有写入任何文件，但返回byte[]</param>
        /// <returns></returns>
        Task<byte[]> WriteAsync<T>(List<T> listData, Dictionary<string, string> column, string fileName = "");

        /// <summary>
        /// 写入 byte 根据实体指定的特性
        /// </summary>
        byte[] WriteByAttribute<T>(List<T> listData, string fileName = "") where T : class;

        /// <summary>
        /// 写入 byte 根据实体指定的特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listData">数据源</param>
        /// <param name="fileName">如果没有文件名，没有写入任何文件，但返回byte[]</param>
        /// <returns></returns>
        Task<byte[]> WriteByAttributeAsync<T>(List<T> listData, string fileName = "") where T : class;

        /// <summary>
        /// 获取列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Dictionary<string, string> GetHeader<T>() where T : class;

        /// <summary>
        /// 获取CSV主体内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listData">数据源</param>
        /// <param name="column">列</param>
        /// <returns></returns>
        StringBuilder GetContent<T>(List<T> listData, Dictionary<string, string> column);
    }
}
