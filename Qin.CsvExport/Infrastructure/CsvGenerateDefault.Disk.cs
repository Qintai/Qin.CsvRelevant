namespace Qin.CsvRelevant
{
    using Dapper;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    internal partial class CsvGenerateDefault
	{
		public async Task WritePhysicalFile<T>(string path, WritePhysicalFileDbConfig config, Action<T>? action = null) where T : class
		{
			config.Check();
			await config.DbConnection.OpenAsync();
			var reader = config.DbConnection.ExecuteReader(config.Sql);
			var rowParser = reader.GetRowParser<T>();

            Dictionary<string, string> column = GetHeader<T>();
            StringBuilder stringbuilder = BuildStringBuilder(new List<T>(), column);
            (int headLength, path) = await WriteHead<T>(path, stringbuilder);

            while (reader.Read())
			{
				T model = rowParser(reader);
				if (action != null)
					action(model);
				await WriteContent<T>(path, stringbuilder, headLength, model, column);
			}

			DelTempFile(path);
		}

		public async Task WritePhysicalFile<T>(string path, IDataReader reader, Func<IDataReader, T> func) where T : class
		{
			Dictionary<string, string> column = GetHeader<T>();
			StringBuilder stringbuilder = BuildStringBuilder(new List<T>(), column);
			(int headLength, path) = await WriteHead<T>(path, stringbuilder);

			while (reader.Read())
			{
				T model = func(reader);
				await WriteContent<T>(path, stringbuilder, headLength, model, column);
			}

			DelTempFile(path);
		}

		private async Task<(int A, string B)> WriteHead<T>(string path, StringBuilder stringbuilder) where T : class
		{
			if (File.Exists(path))
			{
				File.Delete(path);
				Debug.WriteLine($"{path}; Exists, just deleted");
			}

            path += ".temp";
			using StreamWriter streamWriter = new StreamWriter(path, append: false);
			int headLength = stringbuilder.Length;
			char[] charHeadArr = new char[stringbuilder.Length];
			stringbuilder.CopyTo(0, charHeadArr, 0, stringbuilder.Length);
			await streamWriter.WriteAsync(charHeadArr, 0, charHeadArr.Length); // Write Head
			stringbuilder.Clear();

			streamWriter.Flush();
			streamWriter.Close();

			return (headLength, path);
		}

		private async Task WriteContent<T>(string path, StringBuilder stringbuilder, int headLength, T model, Dictionary<string, string> column)
		{
			stringbuilder = BuildStringBuilder(new List<T>() { model }, column);
			char[] charArr = new char[stringbuilder.Length - headLength];
			stringbuilder.CopyTo(headLength, charArr, 0, stringbuilder.Length - headLength);

			using StreamWriter streamWriter2 = new StreamWriter(path, append: true);
			await streamWriter2.WriteAsync(charArr, 0, charArr.Length);

			stringbuilder.Clear();
			streamWriter2.Flush();
			streamWriter2.Close();
		}

		private void DelTempFile(string path)
		{
			if (File.Exists(path))
			{
				var newPath = path.Replace(".temp", "");
				File.Move(path, newPath);
				Debug.WriteLine($"{newPath}; File generated successfully");
			}
		}

	}
}
