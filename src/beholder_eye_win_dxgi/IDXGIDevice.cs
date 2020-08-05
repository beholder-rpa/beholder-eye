namespace beholder_eye_win.DXGI
{
    using SharpGen.Runtime;

    public partial class IDXGIDevice
    {
        /// <inheritdoc/>
        protected override unsafe void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseDevice();
            }
            base.Dispose(disposing);
        }

        private void ReleaseDevice()
        {
            if (Adapter__ != null)
            {
                // Don't use Dispose() in order to avoid circular references
                ((IUnknown)Adapter__).Release();
                Adapter__ = null;
            }
        }

        public Result QueryResourceResidency(IUnknown[] resources, Residency[] residencyStatus)
        {
            return QueryResourceResidency(resources, residencyStatus, resources.Length);
        }
    }
}
