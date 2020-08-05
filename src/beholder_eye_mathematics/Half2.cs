﻿namespace beholder_eye_mathematics
{
    using beholder_eye_mathematics.PackedVector;
    using System;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Packed vector type containing two 16-bit floating-point values.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Half2 : IPackedVector<uint>, IEquatable<Half2>, IFormattable
    {
        [FieldOffset(0)]
        private uint _packedValue;

        /// <summary>
        /// Gets or sets the X component of the vector.
        /// </summary>
        [FieldOffset(0)]
        public Half X;

        /// <summary>
        /// Gets or sets the Y component of the vector.
        /// </summary>
        [FieldOffset(2)]
        public Half Y;

        /// <summary>
        /// Gets or sets the packed value of this <see cref="Half2"/> structure. 
        /// </summary>
        public uint PackedValue
        {
            get => _packedValue;
            set => _packedValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Half2"/> structure.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        public Half2(Half x, Half y)
        {
            _packedValue = 0;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Half2"/> structure.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        public Half2(float x, float y)
        {
            _packedValue = 0;
            X = new Half(x);
            Y = new Half(y);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SharpDX.Half2" /> structure.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        public Half2(ushort x, ushort y)
        {
            _packedValue = 0;
            X = new Half(x);
            Y = new Half(y);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SharpDX.Half2" /> structure.
        /// </summary>
        /// <param name="value">The value to set for both the X and Y components.</param>
        public Half2(Half value)
        {
            _packedValue = 0;
            X = value;
            Y = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SharpDX.Half2" /> structure.
        /// </summary>
        /// <param name="value">Value to initialize X and Y components with.</param>
        public Half2(float value)
        {
            _packedValue = 0;
            X = new Half(value);
            Y = new Half(value);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector2"/> to <see cref="Half2"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Half2(Vector2 value) => new Half2(value.X, value.Y);

        /// <summary>
        /// Performs an explicit conversion from <see cref="Half2"/> to <see cref="Vector2"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Vector2(Half2 value) => new Vector2(value.X, value.Y);

        /// <summary>
        /// Expands this <see cref="Half2"/> structure to a <see cref="Vector2"/>.
        /// </summary>
        public Vector2 ToVector2()
        {
            return new Vector2(
                HalfUtils.ConvertHalfToFloat((ushort)PackedValue),
                HalfUtils.ConvertHalfToFloat((ushort)(PackedValue >> 16))
                );
        }

        #region IPackedVector Implementation
        Vector4 IPackedVector.ToVector4()
        {
            var vector = ToVector2();
            return new Vector4(vector.X, vector.Y, 0.0f, 1.0f);
        }

        void IPackedVector.PackFromVector4(Vector4 vector)
        {
            X = new Half(vector.X);
            Y = new Half(vector.Y);
        }
        #endregion IPackedVector Implementation

        /// <inheritdoc/>
		public override bool Equals(object obj) => obj is Half2 value && Equals(ref value);

        /// <summary>
        /// Determines whether the specified <see cref="Half2"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Half2"/> to compare with this instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Half2 other) => Equals(ref other);

        /// <summary>
		/// Determines whether the specified <see cref="Half2"/> is equal to this instance.
		/// </summary>
		/// <param name="other">The <see cref="Half2"/> to compare with this instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ref Half2 other)
        {
            return X.Equals(ref other.X)
                && Y.Equals(ref other.Y);
        }

        /// <summary>
        /// Compares two <see cref="Half2"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Half2"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="Half2"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Half2 left, Half2 right) => left.Equals(ref right);

        /// <summary>
        /// Compares two <see cref="Half2"/> objects for inequality.
        /// </summary>
        /// <param name="left">The <see cref="Half2"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="Half2"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Half2 left, Half2 right) => !left.Equals(ref right);

        /// <inheritdoc/>
		public override int GetHashCode() => PackedValue.GetHashCode();

        /// <inheritdoc/>
        public override string ToString()
        {
            var vector = ToVector2();
            return $"{nameof(Half2)}({vector.X}, {vector.Y})";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            var vector = ToVector2();
            return $"{nameof(Half2)}({vector.X.ToString(format, formatProvider)}, {vector.Y.ToString(format, formatProvider)})";
        }

        private static uint Pack(float x, float y)
        {
            uint packX = HalfUtils.ConvertFloatToHalf(x);
            uint packY = (uint)(HalfUtils.ConvertFloatToHalf(y) << 16);
            return packX | packY;
        }
    }
}
