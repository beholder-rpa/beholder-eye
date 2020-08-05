namespace beholder_eye_tests
{
    using beholder_eye;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Xunit;

    public class DesktopDuplicatorTests
    {
        private readonly Mock<ILogger<DesktopDuplicator>> _mockLogger;

        public DesktopDuplicatorTests()
        {
            _mockLogger = new Mock<ILogger<DesktopDuplicator>>();
            _mockLogger
                .Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()))
                .Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>((level, eventId, obj, ex, fn) => {
                    Console.WriteLine($"{level} - {obj} {ex}");
                });
        }

        [Fact]
        public void CanCaptureDesktop()
        {
            using var cts = new CancellationTokenSource();
            var desktopDuplicator = new DesktopDuplicator(_mockLogger.Object);
            foreach(var desktopFrame in desktopDuplicator.DuplicateDesktop(cts.Token))
            {
                Assert.NotNull(desktopFrame);
                Assert.False(desktopFrame.IsDesktopImageBufferEmpty);
                cts.Cancel();
            }

            Assert.True(true);
        }

        [Fact]
        public void CanDecodeDesktopFrame()
        {
            var mapJson = File.ReadAllText("./mocks/alignmentmap.json");
            var map = JsonConvert.DeserializeObject<IList<int>>(mapJson);

            using var cts = new CancellationTokenSource();
            var desktopDuplicator = new DesktopDuplicator(_mockLogger.Object);

            var settings = new MatrixSettings()
            {
                Map = map,
                DataFormat = DataMatrixFormat.Raw
            };

            foreach (var desktopFrame in desktopDuplicator.DuplicateDesktop(cts.Token))
            {
                var frame = desktopFrame.DecodeMatrixFrame(settings);
                if (frame != null)
                {
                    cts.Cancel();
                }
            }

            Assert.True(true);
        }

        [Fact]
        public void CanCreateThumbnailFromDesktopFrame()
        {
            var mapJson = File.ReadAllText("./mocks/alignmentmap.json");
            var map = JsonConvert.DeserializeObject<IList<int>>(mapJson);

            using var cts = new CancellationTokenSource();
            var desktopDuplicator = new DesktopDuplicator(_mockLogger.Object);

            var settings = new MatrixSettings()
            {
                Map = map,
                DataFormat = DataMatrixFormat.Raw
            };

            foreach (var desktopFrame in desktopDuplicator.DuplicateDesktop(cts.Token))
            {
                var width = (int)Math.Ceiling(desktopFrame.DesktopWidth * 0.15);
                var height = (int)Math.Ceiling(desktopFrame.DesktopHeight * 0.15);

                var thumbby = desktopFrame.GetThumbnailImage(width, height);
                Assert.True(thumbby.Length > 0);
                cts.Cancel();
            }

            Assert.True(true);
        }

        [Fact]
        public void CanGetPointerImageFromDesktopFrame()
        {
            var mapJson = File.ReadAllText("./mocks/alignmentmap.json");
            var map = JsonConvert.DeserializeObject<IList<int>>(mapJson);

            using var cts = new CancellationTokenSource();
            var desktopDuplicator = new DesktopDuplicator(_mockLogger.Object);

            var settings = new MatrixSettings()
            {
                Map = map,
                DataFormat = DataMatrixFormat.Raw
            };

            foreach (var desktopFrame in desktopDuplicator.DuplicateDesktop(cts.Token))
            {
                var pointerImage = desktopFrame.GetPointerImage();

                //Erm, move the mouse around to get an updated cursor image.
                if (pointerImage != null)
                {
                    Assert.StartsWith("data:image/png;base64,", pointerImage);
                    cts.Cancel();
                }
            }

            Assert.True(true);
        }
    }
}
