namespace beholder_eye_benchmarks
{
    using BenchmarkDotNet.Running;

    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<DesktopFrameBenchmarks>();
            //BenchmarkRunner.Run<DesktopDuplicatorBenchmarks>();
        }
    }
}
