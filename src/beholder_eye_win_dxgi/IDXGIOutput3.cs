namespace beholder_eye_win.DXGI
{
    using SharpGen.Runtime;
    using System;

    public partial class IDXGIOutput3
    {
        /// <summary>
        /// Checks for overlay support.
        /// </summary>
        /// <param name="format">A <see cref="Format"/> value for the color format.</param>
        /// <param name="concernedDevice">Instance of Direct3D device interface.</param>
        /// <returns>Overlay support flags.</returns>
        public OverlaySupportFlags CheckOverlaySupport(Format format, IUnknown concernedDevice)
        {
            if (PlatformDetection.IsUAP)
            {
                throw new NotSupportedException("IDXGIOutput3.CheckOverlaySupport is not supported on UAP platform");
            }

            return CheckOverlaySupport_(format, concernedDevice);
        }
    }
}
