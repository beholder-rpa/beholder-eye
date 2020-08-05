namespace beholder_eye_win.DXGI
{
    using System;
    using System.Runtime.CompilerServices;
    public partial class IDXGISwapChain4
    {
        public unsafe void SetHDRMetaData<T>(HdrMetadataType type, T data) where T : struct
        {
            SetHDRMetaData(type, Unsafe.SizeOf<T>(), new IntPtr(Unsafe.AsPointer(ref data)));
        }

        public unsafe void SetHDRMetaData<T>(HdrMetadataType type, ref T data) where T : struct
        {
            SetHDRMetaData(type, Unsafe.SizeOf<T>(), new IntPtr(Unsafe.AsPointer(ref data)));
        }

        public unsafe void SetHDRMetaData(HdrMetadataType type, HdrMetadataHdr10 data)
        {
            SetHDRMetaData(type, Unsafe.SizeOf<HdrMetadataHdr10>(), new IntPtr(Unsafe.AsPointer(ref data)));
        }
    }
}
