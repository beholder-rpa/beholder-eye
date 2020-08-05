namespace beholder_eye_win.DXGI
{
    using SharpGen.Runtime;
    using System;

    public partial class IDXGIOutput5
    {
        public IDXGIOutputDuplication DuplicateOutput1(IUnknown device, params Format[] supportedFormats)
        {
            if (PlatformDetection.IsUAP)
            {
                throw new NotSupportedException("IDXGIOutput5.DuplicateOutput1 is not supported on UAP platform");
            }

            return DuplicateOutput1_(device, 0, supportedFormats.Length, supportedFormats);
        }
    }
}
