namespace beholder_eye_benchmarks
{
    using beholder_eye;
    using BenchmarkDotNet.Attributes;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using System.Threading;

    public class DesktopDuplicatorBenchmarks
    {
        private Mock<ILogger<DesktopDuplicator>> _mockLogger;
        private DesktopDuplicator _desktopDuplicator;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _mockLogger = new Mock<ILogger<DesktopDuplicator>>();
            _mockLogger
                .Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()))
                .Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>((level, eventId, obj, ex, fn) => {
                    Console.WriteLine($"{level} - {obj} {ex}");
                });
            _desktopDuplicator = new DesktopDuplicator(_mockLogger.Object);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _mockLogger = null;
            _desktopDuplicator = null;
        }

        [Benchmark]
        public void CaptureDesktopOneFrame()
        { 
            using var cts = new CancellationTokenSource();
    
            foreach(var desktopFrame in _desktopDuplicator.DuplicateDesktop(cts.Token))
            {
                cts.Cancel();
            }
        }

        //[Benchmark]
        public void CaptureDesktopTenFrames()
        {
            using var cts = new CancellationTokenSource();
            var counter = 0;
            foreach (var desktopFrame in _desktopDuplicator.DuplicateDesktop(cts.Token))
            {
                if (counter++ > 10)
                {
                    cts.Cancel();
                }
            }
        }

        //[Benchmark]
        public void CaptureDesktopOneHundredFrames()
        {
            using var cts = new CancellationTokenSource();
            var counter = 0;
            foreach (var desktopFrame in _desktopDuplicator.DuplicateDesktop(cts.Token))
            {
                if (counter++ > 100)
                {
                    cts.Cancel();
                }
            }
        }
    }
}
