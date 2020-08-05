namespace beholder_eye_win.Direct3D11
{
    using System;
    using SharpGen.Runtime.Win32;
    using System.Threading;

    public partial class ID3D11Fence
    {
        private const int GENERIC_ALL = 0x10000000;

        public IntPtr CreateSharedHandle(SecurityAttributes? attributes, string name)
        {
            return CreateSharedHandle(attributes, GENERIC_ALL, name);
        }

        public void SetEventOnCompletion(long value, EventWaitHandle waitHandle)
        {
            SetEventOnCompletion(value, waitHandle.SafeWaitHandle.DangerousGetHandle());
        }
    }
}
