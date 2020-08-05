namespace beholder_eye
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an object that performs screen region captures using Graphics.CopyFromScreen
    /// </summary>
    public class ScreenRegionLogger
    {
        private bool _run;
        private readonly uint _displayIndex;

        public event EventHandler<byte[]> OnCapture;

        public int Size { get; private set; }

        public ScreenRegionLogger(uint displayIndex)
        {
            _displayIndex = displayIndex;
        }

        public void Start()
        {
            _run = true;

            var display = new NativeMethods.DISPLAY_DEVICE
            {
                cb = Marshal.SizeOf(typeof(NativeMethods.DISPLAY_DEVICE))
            };

            try
            {
                NativeMethods.EnumDisplayDevices(null, _displayIndex, ref display, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }

            var dm = new NativeMethods.DEVMODE
            {
                dmSize = (short)Marshal.SizeOf(typeof(NativeMethods.DEVMODE))
            };

            NativeMethods.EnumDisplaySettings(display.DeviceName, -1, ref dm);
            Task.Factory.StartNew(() =>
            {
                while (_run)
                {
                    using var bitmap = new Bitmap(dm.dmPelsWidth, dm.dmPelsHeight);
                    using var g = Graphics.FromImage(bitmap);
                    g.CopyFromScreen(dm.dmPositionX, dm.dmPositionY, 0, 0, bitmap.Size);
                    using var ms = new MemoryStream();
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    OnCapture?.Invoke(this, ms.ToArray());
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }

        public void Stop()
        {
            _run = false;
        }
    }
}
