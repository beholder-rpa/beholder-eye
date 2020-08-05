namespace beholder_eye_win.Direct3D11
{
    using System.Threading;

    public partial class ID3D11Device4
    {
        public int RegisterDeviceRemovedEvent(EventWaitHandle waitHandle)
        {
            return RegisterDeviceRemovedEvent(waitHandle.SafeWaitHandle.DangerousGetHandle());
        }
    }
}
