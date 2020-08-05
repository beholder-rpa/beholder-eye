namespace beholder_eye
{
    using System.Drawing;

    /// <summary>
    /// Describes the movement of an image rectangle within a desktop frame.
    /// </summary>
    /// <remarks>
    /// Move regions are always non-stretched regions so the source is always the same size as the destination.
    /// </remarks>
    public struct MovedRegion : System.IEquatable<MovedRegion>
    {
        /// <summary>
        /// Gets the location from where the operating system copied the image region.
        /// </summary>
        public Point Source { get; internal set; }

        /// <summary>
        /// Gets the target region to where the operating system moved the image region.
        /// </summary>
        public Rectangle Destination { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj is MovedRegion otherRegion)
            {
                return Equals(otherRegion);
            }

            return false;
        }

        public bool Equals(MovedRegion other)
        {
            return Source == other.Source && Destination == other.Destination;
        }

        public override int GetHashCode()
        {
            return Source.GetHashCode() + Destination.GetHashCode();
        }

        public static bool operator ==(MovedRegion left, MovedRegion right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MovedRegion left, MovedRegion right)
        {
            return !(left == right);
        }
    }
}
