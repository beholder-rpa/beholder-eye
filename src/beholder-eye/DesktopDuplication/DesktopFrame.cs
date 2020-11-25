namespace beholder_eye
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.IO;
    using Point = System.Drawing.Point;
    using Rectangle = System.Drawing.Rectangle;

    /// <summary>
    /// Provides image data, cursor data, and image metadata about the retrieved desktop frame.
    /// </summary>
    public sealed class DesktopFrame
    {
        public DesktopFrame()
        {
            PointerPosition = new PointerPosition();
            PointerShape = new PointerShape();
        }

        /// <summary>
        /// Gets the buffer representing the last retrieved desktop frame. This image spans the entire bounds of the specified monitor.
        /// </summary>
        internal byte[] DesktopFrameBuffer { get; set; }

        /// <summary>
        /// Gets the buffer containing a 32bit argb bitmap representing the last retrieved pointer.
        /// </summary>
        internal byte[] PointerShapeBuffer { get; set; }

        /// <summary>
        /// Gets the desktop width
        /// </summary>
        public int DesktopWidth { get; internal set; }

        /// <summary>
        /// Gets the desktop height
        /// </summary>
        public int DesktopHeight { get; internal set; }

        /// <summary>
        /// Gets a list of the rectangles of pixels in the desktop image that the operating system moved to another location within the same image.
        /// </summary>
        /// <remarks>
        /// To produce a visually accurate copy of the desktop, an application must first process all moved regions before it processes updated regions.
        /// </remarks>
        public IList<MovedRegion> MovedRegions { get; internal set; }

        /// <summary>
        /// Returns the list of non-overlapping rectangles that indicate the areas of the desktop image that the operating system updated since the last retrieved frame.
        /// </summary>
        /// <remarks>
        /// To produce a visually accurate copy of the desktop, an application must first process all moved regions before it processes updated regions.
        /// </remarks>
        public IList<Rectangle> UpdatedRegions { get; internal set; }

        /// <summary>
        /// The number of frames that the operating system accumulated in the desktop image surface since the last retrieved frame.
        /// </summary>
        public int AccumulatedFrames { get; internal set; }

        /// <summary>
        /// Gets the current position of the pointer on the image
        /// </summary>
        public PointerPosition PointerPosition { get; internal set; }

        /// <summary>
        /// Gets the information representing the current pointer.
        /// </summary>
        public PointerShape PointerShape { get; internal set; }

        /// <summary>
        /// Gets whether the desktop image contains protected content that was already blacked out in the desktop image.
        /// </summary>
        public bool ProtectedContentMaskedOut { get; internal set; }

        /// <summary>
        /// Gets whether the operating system accumulated updates by coalescing updated regions. If so, the updated regions might contain unmodified pixels.
        /// </summary>
        public bool RectanglesCoalesced { get; internal set; }

        /// <summary>
        /// Gets a value that indicates if the desktop image is completely black.
        /// </summary>
        /// <returns></returns>
        public bool IsDesktopImageBufferEmpty { get; internal set; }

        private static Point FindBlock(ReadOnlySpan<byte> span, int startX, int startY, int height, int width, int bpp, int size)
        {
            bool IsContiguousColorAtPoint(ReadOnlySpan<byte> span, int x, int y, int length)
            {
                for (int i = 0; i < length; i++)
                {
                    var ix = y * width * bpp + x * bpp + i * bpp;
                    var b = span[ix];
                    var g = span[ix + 1];
                    var r = span[ix + 2];
                    if (r != 0 || g != 255 || b != 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            // Find the first 0,255,0 block of the specified size on the image.
            for (int y = startY; y < height - size - 1; y++)
            {
                for (int x = startX; x < width - size - 1; x++)
                {
                    if (IsContiguousColorAtPoint(span, x, y, size))
                    {
                        var match = true;
                        for (int y1 = y + 1; y1 <= y + size + 1; y1++)
                        {
                            if (!IsContiguousColorAtPoint(span, x, y1, size))
                            {
                                match = false;
                            }
                        }

                        if (!match)
                        {
                            break;
                        }

                        return new Point(x, y);
                    }
                }
            }

            return Point.Empty;
        }

        public IList<Point> GenerateAlignmentMap(int pixelSize)
        {
            if (DesktopFrameBuffer == null || DesktopFrameBuffer.Length <= 0 || IsDesktopImageBufferEmpty)
            {
                return null;
            }

            var bpp = System.Drawing.Image.GetPixelFormatSize(PixelFormat.Format32bppRgb) / 8;

            unsafe
            {
                var span = new ReadOnlySpan<byte>(DesktopFrameBuffer);

                var pixels = new List<Point>();

                var currentX = 0;
                var currentY = 0;
                Point lastBlock;
                while (currentY < DesktopHeight - pixelSize - 1)
                {
                    lastBlock = FindBlock(span, currentX, currentY, DesktopHeight, DesktopWidth, bpp, pixelSize);
                    if (lastBlock != Point.Empty)
                    {
                        pixels.Add(lastBlock);

                        currentX = lastBlock.X + pixelSize + 1;
                        currentY = lastBlock.Y;
                    }
                    else
                    {
                        // If didn't find a block, and we haven't found any pixels, uh, there are none. Break.
                        if (pixels.Count == 0)
                        {
                            break;
                        }

                        // If we didn't find a block and If the line we're currently on is already beyond the last found pixel, there are no more. Break.
                        if (currentY >= pixels[pixels.Count - 1].Y + pixelSize + 1)
                        {
                            break;
                        }

                        // Otherwise, reset to the start of the line at the last found pixel.
                        currentX = 0;
                        currentY = pixels[pixels.Count - 1].Y + pixelSize + 1;
                    }
                }
                return pixels;
            }
        }

        public MatrixFrame DecodeMatrixFrame(MatrixSettings settings)
        {
            if (DesktopFrameBuffer == null || DesktopFrameBuffer.Length <= 0 || IsDesktopImageBufferEmpty)
            {
                return null;
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (settings.Map == null || settings.Map.Count == 0 || settings.Map.Count % 2 != 0)
            {
                throw new InvalidOperationException("The matrix map specified in the settings must be non-null and contain at least 1 point pair and have an even number of values.");
            }

            var rawData = new byte[settings.Map.Count / 2 * 3];

            var span = new ReadOnlySpan<byte>(DesktopFrameBuffer);
            var bpp = System.Drawing.Image.GetPixelFormatSize(PixelFormat.Format32bppRgb) / 8;

            for (int i = 0; i < settings.Map.Count / 2; i++)
            {
                var x = settings.Map[i * 2];
                var y = settings.Map[i * 2 + 1];
                var ix = y * DesktopWidth * bpp + x * bpp;
                rawData[i * 3] = span[ix + 2];
                rawData[i * 3 + 1] = span[ix + 1];
                rawData[i * 3 + 2] = span[ix];
            }

            return MatrixFrame.CreateMatrixFrame(rawData, settings);
        }

        /// <summary>
        /// Creates a base64 encoded thumbnail image of the current desktop image with the specified width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public byte[] GetThumbnailImage(int width, int height)
        {
            if (DesktopFrameBuffer == null || DesktopFrameBuffer.Length <= 0 || IsDesktopImageBufferEmpty)
            {
                return null;
            }

            var span = new ReadOnlySpan<byte>(DesktopFrameBuffer);
            using var ms = new MemoryStream();
            using var image = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(span, DesktopWidth, DesktopHeight);
            {
                image.Mutate(x => x
                     .Resize(width, height));

                image.SaveAsPng(ms);
            }
            return ms.ToArray();
        }

        public byte[] GetSnapshot(int width, int height, SnapshotFormat format)
        {
            if (DesktopFrameBuffer == null || DesktopFrameBuffer.Length <= 0 || IsDesktopImageBufferEmpty)
            {
                return null;
            }

            var span = new ReadOnlySpan<byte>(DesktopFrameBuffer);
            using var ms = new MemoryStream();
            using var image = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(span, DesktopWidth, DesktopHeight);
            {
                image.Mutate(x => x
                     .Resize(width, height));

                switch (format)
                {
                    case SnapshotFormat.Jpeg:
                        image.SaveAsJpeg(ms);
                        break;
                    case SnapshotFormat.Png:
                    default:
                        image.SaveAsPng(ms);
                        break;
                }
            }
            return ms.ToArray();
        }

        public byte[] GetPointerImage()
        {
            if (PointerShapeBuffer == null)
            {
                return null;
            }

            if (PointerShapeBuffer.Length != PointerShape.Width.Value * PointerShape.Height.Value * 4)
            {
                return null;
            }

            var span = new ReadOnlySpan<byte>(PointerShapeBuffer);
            using var ms = new MemoryStream();
            using var image = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(span, PointerShape.Width.Value, PointerShape.Height.Value);
            {
                image.SaveAsPng(ms);
            }

            return ms.ToArray();
        }

        public static DesktopFrame FromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var result = new DesktopFrame();

            using var image = SixLabors.ImageSharp.Image.Load(path);
            {
                using var imageClone = image.CloneAs<Bgra32>();
                {
                    var bpp = imageClone.PixelType.BitsPerPixel / 8;
                    int bytes = imageClone.Height * imageClone.Width * bpp;

                    var frameBuffer = new byte[bytes];
                    var bufferSpan = new Span<byte>(frameBuffer);
                    if (imageClone.TryGetSinglePixelSpan(out Span<Bgra32> imageSpan))
                    {
                        for (int i = 0; i < imageSpan.Length; i++)
                        {
                            bufferSpan[i * bpp] = imageSpan[i].B;
                            bufferSpan[i * bpp + 1] = imageSpan[i].G;
                            bufferSpan[i * bpp + 2] = imageSpan[i].R;
                            bufferSpan[i * bpp + 3] = imageSpan[i].A;
                        }

                        result.DesktopFrameBuffer = frameBuffer;
                        result.DesktopWidth = imageClone.Width;
                        result.DesktopHeight = imageClone.Height;
                        result.IsDesktopImageBufferEmpty = bufferSpan.Trim((byte)0x00).IsEmpty;
                    }
                }
            }

            return result;
        }
    }
}
