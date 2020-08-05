namespace beholder_eye_win.DXGI
{
    using SharpGen.Runtime;
    using System;

    public partial class IDXGIObject
    {
        public T GetParent<T>() where T : ComObject
        {
            if (GetParent(typeof(T).GUID, out IntPtr nativePtr).Failure)
            {
                return default;
            }

            return FromPointer<T>(nativePtr);
        }
    }
}
