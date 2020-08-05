namespace beholder_eye_benchmarks
{
    using beholder_eye;
    using BenchmarkDotNet.Attributes;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;

    public class DesktopFrameBenchmarks
    {
        private DesktopFrame AlignmentFrame;
        private DesktopFrame AlphaFrame;
        private DesktopFrame DataFrame;
        private DesktopFrame TestFrame;
        private IList<int> AlignmentMap;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var mapJson = File.ReadAllText("./mocks/alignmentmap.json");
            AlignmentMap = JsonSerializer.Deserialize<IList<int>>(mapJson);

            AlignmentFrame = DesktopFrame.FromFile("./mocks/alignpattern.bmp");
            AlphaFrame = DesktopFrame.FromFile("./mocks/alphapattern.bmp");
            DataFrame = DesktopFrame.FromFile("./mocks/datapattern.bmp");
            TestFrame = DesktopFrame.FromFile("./mocks/testpattern.bmp");

        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            AlignmentMap = null;

            AlignmentFrame = null;
            AlphaFrame = null;
            DataFrame = null;
            TestFrame = null;
        }

        [Benchmark]
        public void GenerateAlignmentMap()
        {
            AlignmentFrame.GenerateAlignmentMap(2);
        }

        [Benchmark]
        public void DecodeAlphaFrame()
        {
            AlphaFrame.DecodeMatrixFrame(new MatrixSettings()
            {
                Map = AlignmentMap,
                DataFormat = DataMatrixFormat.Text,
            });
        }

        [Benchmark]
        public void DecodeMatrixFrame()
        {
            DataFrame.DecodeMatrixFrame(new MatrixSettings()
            {
                Map = AlignmentMap,
                DataFormat = DataMatrixFormat.Json,
            });
        }

        [Benchmark]
        public void DecodeTestFrame()
        {
            TestFrame.DecodeMatrixFrame(new MatrixSettings()
            {
                Map = AlignmentMap,
                DataFormat = DataMatrixFormat.Raw,
            });
        }

        [Benchmark]
        public void GetThumbnailImage()
        {
            DataFrame.GetThumbnailImage((int)Math.Ceiling(DataFrame.DesktopWidth * 0.15), (int)Math.Ceiling(DataFrame.DesktopHeight * 0.15));
        }
    }
}