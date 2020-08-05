namespace beholder_eye_tests
{
    using beholder_eye;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using Xunit;

    public class DesktopFrameTests
    {
        private const string TestPattern = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        [Fact]
        public void CanGenerateAlignmentMap()
        {
            var foo = DesktopFrame.FromFile("./mocks/alignpattern.bmp");
            var alignmentMap = foo.GenerateAlignmentMap(2);

            Assert.Equal(156 * 46, alignmentMap.Count);

            var flatPoints = new List<int>(alignmentMap.Count * 2);
            foreach (var point in alignmentMap)
            {
                flatPoints.Add(point.X);
                flatPoints.Add(point.Y);
            }

            //var map = JsonSerializer.Serialize(flatPoints);
            //File.WriteAllText("map.json", map);
        }

        [Fact]
        public void CanDecodeAlphaPattern()
        {
            var mapJson = File.ReadAllText("./mocks/alignmentmap.json");
            var map = JsonSerializer.Deserialize<IList<int>>(mapJson);

            var foo = DesktopFrame.FromFile("./mocks/alphapattern.bmp");

            var settings = new MatrixSettings()
            {
                Map = map,
                DataFormat = DataMatrixFormat.Text,
            };
            var dataMatrix = foo.DecodeMatrixFrame(settings);
            var matrixJson = JsonSerializer.Serialize(dataMatrix);
            dataMatrix = JsonSerializer.Deserialize<MatrixFrame>(matrixJson);
            var testData = dataMatrix.Data.ToString();

            Assert.Equal((map.Count / 2) * 3 - 6, testData.Length);

            Assert.Equal(120, dataMatrix.FrameId);

            int width = 156;
            int height = 46;
            Assert.Equal(width, dataMatrix.Metadata[0]);
            Assert.Equal(height, dataMatrix.Metadata[1]);

            var frameType = dataMatrix.Metadata[2] >> 4;
            var pixelSize = dataMatrix.Metadata[2] % 16;

            Assert.Equal(2, frameType);
            Assert.Equal(2, pixelSize);

            var errors = 0;
            for (int i = 0; i < testData.Length; i++)
            {
                if (testData[i] != TestPattern[i % TestPattern.Length])
                {
                    errors++;
                }
            }

            Assert.Equal(0, errors);
        }

        [Fact]
        public void CanDecodeTestPattern()
        {
            var mapJson = File.ReadAllText("./mocks/alignmentmap.json");
            var map = JsonSerializer.Deserialize<IList<int>>(mapJson);

            var foo = DesktopFrame.FromFile("./mocks/testpattern.bmp");

            var settings = new MatrixSettings()
            {
                Map = map,
                DataFormat = DataMatrixFormat.Raw,
            };
            var dataMatrix = foo.DecodeMatrixFrame(settings);
            var matrixJson = JsonSerializer.Serialize(dataMatrix);
            dataMatrix = JsonSerializer.Deserialize<MatrixFrame>(matrixJson);
            var testData = JsonSerializer.Deserialize<byte[]>(((JsonElement)dataMatrix.Data).GetRawText());

            Assert.Equal((map.Count / 2) * 3, testData.Length);

            Assert.Equal(119, dataMatrix.FrameId);

            int width = 156;
            int height = 46;
            Assert.Equal(width, dataMatrix.Metadata[0]);
            Assert.Equal(height, dataMatrix.Metadata[1]);

            var frameType = dataMatrix.Metadata[2] >> 4;
            var pixelSize = dataMatrix.Metadata[2] % 16;

            Assert.Equal(1, frameType);
            Assert.Equal(2, pixelSize);

            var errors = 0;

            // Test the full range
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Skip the metadata
                    if (y == 0 && (x == 0 || x == 1))
                    {
                        continue;
                    }

                    var ix = y * width * 3 + x * 3;

                    var x1 = testData[ix];
                    var y1 = testData[ix + 1];
                    var z = testData[ix + 2];

                    if (x + 1 != x1 || y + 1 != y1 || z != 255)
                    {
                        errors++;
                    }
                }
            }

            Assert.Equal(0, errors);
        }

        [Fact]
        public void CanDecodeMatrixEventData()
        {
            var mapJson = File.ReadAllText("./mocks/alignmentmap.json");
            var map = JsonSerializer.Deserialize<IList<int>>(mapJson);

            var foo = DesktopFrame.FromFile("./mocks/datapattern.bmp");

            var settings = new MatrixSettings()
            {
                Map = map,
                DataFormat = DataMatrixFormat.MatrixEvents,
            };
            var dataMatrix = foo.DecodeMatrixFrame(settings);
            var matrixJson = JsonSerializer.Serialize(dataMatrix);
            dataMatrix = JsonSerializer.Deserialize<MatrixFrame>(matrixJson);
            var testData = JsonSerializer.Deserialize<IList<MatrixEvent>>(((JsonElement)dataMatrix.Data).GetRawText());

            Assert.Equal(45, testData.Count);

            Assert.Equal(115, dataMatrix.FrameId);

            int width = 156;
            int height = 46;
            Assert.Equal(width, dataMatrix.Metadata[0]);
            Assert.Equal(height, dataMatrix.Metadata[1]);

            var frameType = dataMatrix.Metadata[2] >> 4;
            var pixelSize = dataMatrix.Metadata[2] % 16;

            Assert.Equal(0, frameType);
            Assert.Equal(2, pixelSize);
            var playerData = (JsonElement)testData.FirstOrDefault(td => td.Topic == "player").Data;
            Assert.Equal("Sleepyhead", playerData.GetProperty("n").GetString());
        }

        [Fact]
        public void CanGetDesktopThumbnail()
        {
            var foo = DesktopFrame.FromFile("./mocks/datapattern.bmp");

            var thumbby = foo.GetThumbnailImage(640, 480);
            Assert.True(thumbby.Length > 0);
        }
    }
}
