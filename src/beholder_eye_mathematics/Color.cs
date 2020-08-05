﻿namespace beholder_eye_mathematics
{
    using beholder_eye_mathematics.PackedVector;
    using System;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a 32-bit RGBA color (4 bytes).
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Color : IPackedVector<uint>, IEquatable<Color>
    {
        [FieldOffset(0)]
        private uint _packedValue;

        /// <summary>
        /// The red component of the color.
        /// </summary>
        [FieldOffset(0)]
        public byte R;

        /// <summary>
        /// The green component of the color.
        /// </summary>
        [FieldOffset(1)]
        public byte G;

        /// <summary>
        /// The blue component of the color.
        /// </summary>
        [FieldOffset(2)]
        public byte B;

        /// <summary>
        /// The alpha component of the color.
        /// </summary>
        [FieldOffset(3)]
        public byte A;

        /// <summary>
        /// Gets or Sets the current color as a packed value.
        /// </summary>
        public uint PackedValue
        {
            get => _packedValue;
            set => _packedValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public Color(byte value)
        {
            _packedValue = 0;
            A = R = G = B = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public Color(float value) : this()
        {
            _packedValue = PackHelpers.PackRGBA(value, value, value, value);
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="Color"/> struct.
		/// </summary>
		/// <param name="r">The red component of the color.</param>
        /// <param name="g">The green component of the color.</param>
        /// <param name="b">The blue component of the color.</param>
        /// <param name="a">The alpha component of the color.</param>
		public Color(byte r, byte g, byte b, byte a)
        {
            _packedValue = 0;
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.  Alpha is set to 255.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        public Color(byte red, byte green, byte blue)
        {
            _packedValue = 0;
            R = red;
            G = green;
            B = blue;
            A = 255;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.  Passed values are clamped within byte range.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        /// <param name="alpha">The alpha component of the color</param>
        public Color(int red, int green, int blue, int alpha)
        {
            _packedValue = 0;
            R = PackHelpers.ToByte(red);
            G = PackHelpers.ToByte(green);
            B = PackHelpers.ToByte(blue);
            A = PackHelpers.ToByte(alpha);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.  Alpha is set to 255.  Passed values are clamped within byte range.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        public Color(int red, int green, int blue)
        {
            _packedValue = 0;
            R = PackHelpers.ToByte(red);
            G = PackHelpers.ToByte(green);
            B = PackHelpers.ToByte(blue);
            A = 255;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="r">Red component.</param>
        /// <param name="g">Green component.</param>
        /// <param name="b">Blue component.</param>
        public Color(float r, float g, float b) : this()
        {
            _packedValue = PackHelpers.PackRGBA(r, g, b, 1.0f);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="r">Red component.</param>
        /// <param name="g">Green component.</param>
        /// <param name="b">Blue component.</param>
        /// <param name="a">Alpha component.</param>
        public Color(float r, float g, float b, float a) : this()
        {
            _packedValue = PackHelpers.PackRGBA(r, g, b, a);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="vector">The red, green, and blue components of the color.</param>
        /// <param name="alpha">The alpha component of the color.</param>
        public Color(Vector3 vector, float alpha) : this()
        {
            _packedValue = PackHelpers.PackRGBA(vector.X, vector.Y, vector.Z, alpha);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="vector">A three-component color.</param>
        public Color(Vector3 vector) : this()
        {
            _packedValue = PackHelpers.PackRGBA(vector.X, vector.Y, vector.Z, 1.0f);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="vector">A four-component color.</param>
        public Color(Vector4 vector) : this()
        {
            _packedValue = PackHelpers.PackRGBA(vector.X, vector.Y, vector.Z, vector.W);
        }


        public void Deconstruct(out byte red, out byte green, out byte blue, out byte alpha)
        {
            red = R;
            green = G;
            blue = B;
            alpha = A;
        }

        /// <summary>
        /// Gets a four-component vector representation for this object.
        /// </summary>
        public Vector4 ToVector4()
        {
            PackHelpers.UnpackRGBA(_packedValue, out var x, out var y, out var z, out var w);
            return new Vector4(x, y, z, w);
        }

        void IPackedVector.PackFromVector4(Vector4 vector)
        {
            _packedValue = PackHelpers.PackRGBA(vector.X, vector.Y, vector.Z, vector.W);
        }

        /// <inheritdoc/>
		public override bool Equals(object obj) => obj is Color color && Equals(ref color);

        /// <summary>
        /// Determines whether the specified <see cref="Color"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Color"/> to compare with this instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Color other) => Equals(ref other);

        /// <summary>
		/// Determines whether the specified <see cref="Color"/> is equal to this instance.
		/// </summary>
		/// <param name="other">The <see cref="Color"/> to compare with this instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ref Color other)
        {
            return R == other.R
                && G == other.G
                && B == other.B
                && A == other.A;
        }

        /// <summary>
        /// Compares two <see cref="Color"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Color"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="Color"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Color left, Color right) => left.Equals(ref right);

        /// <summary>
        /// Compares two <see cref="Color"/> objects for inequality.
        /// </summary>
        /// <param name="left">The <see cref="Color"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="Color"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Color left, Color right) => !left.Equals(ref right);

        /// <inheritdoc/>
		public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = R.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                hashCode = (hashCode * 397) ^ A.GetHashCode();
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"R={R}, G={G}, B={B}, A={A}";
        }
    }
}
