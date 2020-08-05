namespace beholder_eye_win.DXGI
{
    using System.Threading;

    public partial class IDXGIFactory7
    {
        public int RegisterAdaptersChangedEvent(EventWaitHandle waitHandle)
        {
            return RegisterAdaptersChangedEvent(waitHandle.SafeWaitHandle.DangerousGetHandle());
        }
    }
}
