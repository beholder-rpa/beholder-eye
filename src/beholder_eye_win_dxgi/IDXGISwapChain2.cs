namespace beholder_eye_win.DXGI
{
    using System.Drawing;

    public partial class IDXGISwapChain2
    {
        public Size SourceSize
        {
            get
            {
                GetSourceSize(out var width, out var height);
                return new Size(width, height);
            }
            set
            {
                SetSourceSize(value.Width, value.Height);
            }
        }
    }
}
