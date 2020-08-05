namespace beholder_eye_win.Direct3D11
{
    using SharpGen.Runtime;

    public partial class ID3D11On12Device1
    {
        public T GetD3D12Device<T>() where T : ComObject
        {
            var result = GetD3D12Device(typeof(T).GUID, out var devicePtr);
            if (result.Failure)
            {
                return default;
            }

            return FromPointer<T>(devicePtr);
        }
    }
}
