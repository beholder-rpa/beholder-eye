namespace beholder_eye_mathematics.PackedVector
{
    using System.Numerics;

    /// <summary>
    /// Interface that converts packed vector types to and from <see cref="Vector4"/> values.
    /// </summary>
    public interface IPackedVector
    {
        /// <summary>
        /// Converts the packed representation into a <see cref="Vector4"/>.
        /// </summary>
        Vector4 ToVector4();

        /// <summary>Packed value from a <see cref="Vector4"/>.</summary>
        /// <param name="vector">The vector to create the packed representation from.</param>
        void PackFromVector4(Vector4 vector);
    }

    /// <summary>
    /// Converts packed vector types to and from <see cref="Vector4"/> values.
    /// </summary>
    public interface IPackedVector<TPacked> : IPackedVector
    {
        /// <summary>
        /// Gets or Sets the packed representation of the value.
        /// </summary>
        TPacked PackedValue
        {
            get;
            set;
        }
    }
}
