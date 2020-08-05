namespace beholder_eye_win.Direct3D
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public partial class Blob
    {
        public byte[] GetBytes()
        {
            unsafe
            {
                var result = new byte[GetBufferSize()];
                fixed (byte* pResult = result)
                {
                    Unsafe.CopyBlockUnaligned(pResult, (void*)GetBufferPointer(), (uint)result.Length);
                }

                return result;
            }
        }

        public string ConvertToString()
        {
            return Marshal.PtrToStringAnsi(BufferPointer);
        }
    }
}
