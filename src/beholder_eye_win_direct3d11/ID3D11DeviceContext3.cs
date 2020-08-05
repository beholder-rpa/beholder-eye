namespace beholder_eye_win.Direct3D11
{
    using System.Threading;

    public partial class ID3D11DeviceContext3
    {
        public void Flush1(ContextType contextType, EventWaitHandle waitHandle)
        {
            Flush1(contextType, waitHandle.SafeWaitHandle.DangerousGetHandle());
        }
    }
}
