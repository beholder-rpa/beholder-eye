namespace beholder_eye
{
    using Vortice.DXGI;
    using System.Drawing;

    internal class PointerInfo
    {
        public byte[] ShapeBuffer;
        public OutduplPointerShapeInfo ShapeInfo;
        public Point Position;
        public bool Visible;
        public int WhoUpdatedPositionLast;
        public long LastTimeStamp;
    }
}
