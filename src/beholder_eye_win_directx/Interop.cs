namespace beholder_eye_win
{
    using SharpGen.Runtime;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class Interop
    {
        public static void Read<T>(IntPtr source, T[] values) where T : unmanaged
        {
            var count = values.Length;
            fixed (void* dstPtr = values)
            {
                Unsafe.CopyBlockUnaligned(dstPtr, (void*)source, (uint)(count * sizeof(T)));
            }
        }

        public static void Write<T>(IntPtr destination, ref T value) where T : unmanaged
        {
            unsafe
            {
                Unsafe.CopyBlockUnaligned((void*)destination, Unsafe.AsPointer(ref value), (uint)(sizeof(T)));
            }
        }

        public static void Write<T>(IntPtr destination, T[] values) where T : unmanaged
        {
            MemoryHelpers.Write(destination, new Span<T>(values), values.Length);
        }

        public static IntPtr Alloc<T>(int count = 1) where T : unmanaged
        {
            return Marshal.AllocHGlobal(sizeof(T) * count);
        }

        public static IntPtr AllocToPointer<T>(T[] values) where T : unmanaged
        {
            if (values == null
                || values.Length == 0)
            {
                return IntPtr.Zero;
            }

            int structSize = sizeof(T);
            int totalSize = values.Length * structSize;
            var ptr = Marshal.AllocHGlobal(totalSize);

            var walk = (byte*)ptr;
            for (int i = 0; i < values.Length; i++)
            {
                Unsafe.Copy(walk, ref values[i]);
                walk += structSize;
            }

            return ptr;
        }

        /// <summary>
        /// Native memcpy.
        /// </summary>
        /// <param name="dest">The destination memory location.</param>
        /// <param name="src">The source memory location.</param>
        /// <param name="sizeInBytesToCopy">The byte count.</param>
        public static void CopyMemory(IntPtr dest, IntPtr src, int sizeInBytesToCopy)
        {
            unsafe
            {
                Buffer.MemoryCopy(src.ToPointer(), dest.ToPointer(), sizeInBytesToCopy, sizeInBytesToCopy);
            }
        }
    }
}
