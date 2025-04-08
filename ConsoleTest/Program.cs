using BenchmarkDotNet.Running;
using ConsoleTest;

//new ConsoleTest.ExportTest().Export4();

BenchmarkRunner.Run(typeof(ConsoleTest.ExportTest));
