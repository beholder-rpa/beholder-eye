namespace beholder_eye_win.DXGI
{
    using System.Collections.Generic;

    public partial class IDXGIFactory1
    {
        public IDXGIAdapter1[] EnumAdapters1()
        {
            var adapters = new List<IDXGIAdapter1>();
            for (int adapterIndex = 0; EnumAdapters1(adapterIndex, out var adapter) != ResultCode.NotFound; ++adapterIndex)
            {
                adapters.Add(adapter);
            }

            return adapters.ToArray();
        }
    }
}
