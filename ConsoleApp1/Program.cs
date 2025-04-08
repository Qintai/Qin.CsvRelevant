using BenchmarkDotNet.Running;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(ExportTest));
        }
    }
}
