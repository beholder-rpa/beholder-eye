namespace beholder_eye_win.DXGI
{
    using beholder_eye_mathematics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    public partial struct PresentParameters
    {
        /// <summary>	
        /// <para>A list of updated rectangles that you update in the back buffer for the presented frame. An application must update every single pixel in each rectangle that it reports to the runtime; the application cannot assume that the pixels are saved from the previous frame. For more information about updating dirty rectangles, see Remarks. You can set this member to <c>null</c> if DirtyRectsCount is 0. An application must not update any pixel outside of the dirty rectangles.</para>	
        /// </summary>	
        public Rect[] DirtyRectangles;

        /// <summary>	
        /// <para> A reference to the scrolled rectangle. The scrolled rectangle is the rectangle of the previous frame from which the runtime bit-block transfers (bitblts) content. The runtime also uses the scrolled rectangle to optimize presentation in terminal server and indirect display scenarios.</para>	
        ///  <para>The scrolled rectangle also describes the destination rectangle, that is, the region on the current frame that is filled with scrolled content. You can set this member to <c>null</c> to indicate that no content is scrolled from the previous frame.</para>	
        /// </summary>	
        public Rect? ScrollRectangle;

        /// <summary>	
        /// <para>A reference to the offset of the scrolled area that goes from the source rectangle (of previous frame) to the destination rectangle (of current frame). You can set this member to <c>null</c> to indicate no offset.</para>	
        /// </summary>	
        /// <unmanaged>POINT* pScrollOffset</unmanaged>	
        public Point? ScrollOffset;

        // Internal native struct used for marshalling
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        internal struct __Native
        {
            public int DirtyRectsCount;
            public System.IntPtr PDirtyRects;
            public System.IntPtr PScrollRect;
            public System.IntPtr PScrollOffset;
        }
    }
}
