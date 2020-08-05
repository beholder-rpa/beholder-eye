namespace beholder_eye_win.DXGI
{
    public partial struct SampleDescription
    {
        /// <summary>
        /// Create new instance of <see cref="SampleDescription"/> struct.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="quality"></param>
        public SampleDescription(int count, int quality)
        {
            Count = count;
            Quality = quality;
        }

        public override string ToString() => $"Count: {Count}, Quality: {Quality}";
    }
}
