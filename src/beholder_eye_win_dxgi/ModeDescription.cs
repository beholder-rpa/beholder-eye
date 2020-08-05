namespace beholder_eye_win.DXGI
{
    public partial struct ModeDescription
    {
        /// <summary>
        /// Initialize instance of <see cref="ModeDescription"/> struct.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        public ModeDescription(
            int width,
            int height,
            Format format = Format.B8G8R8A8_UNorm)
        {
            Width = width;
            Height = height;
            Format = format;
            RefreshRate = new Rational(60, 1);
            ScanlineOrdering = ModeScanlineOrder.Unspecified;
            Scaling = ModeScaling.Unspecified;
        }
    }
}
