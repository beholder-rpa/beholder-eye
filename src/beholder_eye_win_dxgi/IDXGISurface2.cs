namespace beholder_eye_win.DXGI
{
    using SharpGen.Runtime;

    public partial class IDXGISurface2
    {
        public T GetResource<T>(out int subresourceIndex) where T : ComObject
        {
            if (GetResource(typeof(T).GUID, out var nativePtr, out subresourceIndex).Failure)
            {
                return default;
            }

            return FromPointer<T>(nativePtr);
        }
    }
}
