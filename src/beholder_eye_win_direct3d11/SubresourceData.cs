namespace beholder_eye_win.Direct3D11
{
    using System;

    public partial struct SubresourceData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubresourceData"/> struct.
        /// </summary>
        /// <param name="dataPointer">The dataPointer.</param>
        /// <param name="pitch">The row pitch.</param>
        /// <param name="slicePitch">The slice pitch.</param>
        public SubresourceData(IntPtr dataPointer, int pitch = 0, int slicePitch = 0)
        {
            DataPointer = dataPointer;
            Pitch = pitch;
            SlicePitch = slicePitch;
        }
    }
}
