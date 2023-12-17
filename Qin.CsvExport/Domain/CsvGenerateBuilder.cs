namespace Qin.CsvRelevant
{
    public class CsvGenerateBuilder
    {
        private static ICsvGenerate _instance;
        private static readonly object syn = new object();

        public static ICsvGenerate Build()
        {
            if (_instance == null)
            {
                lock (syn)
                {
                    if (_instance == null)
                    {
                        _instance = new CsvGenerateDefault();
                    }
                }
            }
            return _instance;
        }
    }
}
