namespace beholder_eye_win.DXGI
{
    using System.Collections.Generic;

    public partial class IDXGIFactory
    {
        public IDXGIAdapter[] EnumAdapters()
        {
            var adapters = new List<IDXGIAdapter>();
            for (int adapterIndex = 0; EnumAdapters(adapterIndex, out var adapter) != ResultCode.NotFound; ++adapterIndex)
            {
                adapters.Add(adapter);
            }

            return adapters.ToArray();
        }
    }
}
