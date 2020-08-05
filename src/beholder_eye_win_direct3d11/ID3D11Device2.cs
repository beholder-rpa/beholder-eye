namespace beholder_eye_win.Direct3D11
{
    using beholder_eye_win.DXGI;

    public partial class ID3D11Device2
    {
        public unsafe ID3D11DeviceContext2 CreateDeferredContext2()
        {
            return CreateDeferredContext2(0);
        }

        public int CheckMultisampleQualityLevels1(Format format, int sampleCount)
        {
            return CheckMultisampleQualityLevels1(format, sampleCount, CheckMultisampleQualityLevelsFlags.TiledResource);
        }
    }
}
