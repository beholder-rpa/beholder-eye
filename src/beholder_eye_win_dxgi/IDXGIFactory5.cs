namespace beholder_eye_win.DXGI
{
    using SharpGen.Runtime.Win32;
    using System;
    using System.Runtime.CompilerServices;

    public partial class IDXGIFactory5
    {
        /// <summary>
        /// Gets if tearing is allowed during present.
        /// </summary>
        public unsafe bool PresentAllowTearing
        {
            get
            {
                RawBool allowTearing;
                CheckFeatureSupport(Feature.PresentAllowTearing, new IntPtr(&allowTearing), sizeof(RawBool));
                return allowTearing;
            }
        }

        public unsafe bool CheckFeatureSupport<T>(Feature feature, ref T featureSupport) where T : unmanaged
        {
            return CheckFeatureSupport(feature, (IntPtr)Unsafe.AsPointer(ref featureSupport), sizeof(T)).Success;
        }
    }
}
