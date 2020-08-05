﻿namespace beholder_eye_mathematics
{
    using System;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a three dimensional mathematical int vector.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Int3 : IEquatable<Int3>, IFormattable
    {
        /// <summary>
        /// A <see cref="Int3"/> with all of its components set to zero.
        /// </summary>
        public static readonly Int3 Zero = new Int3();

        /// <summary>
        /// The X unit <see cref="Int2"/> (1, 0, 0).
        /// </summary>
        public static readonly Int3 UnitX = new Int3(1, 0, 0);

        /// <summary>
        /// The Y unit <see cref="Int3"/> (0, 1, 0).
        /// </summary>
        public static readonly Int3 UnitY = new Int3(0, 1, 0);

        /// <summary>
        /// The Y unit <see cref="Int3"/> (0, 0, 1).
        /// </summary>
        public static readonly Int3 UnitZ = new Int3(0, 0, 1);

        /// <summary>
        /// A <see cref="Int3"/> with all of its components set to one.
        /// </summary>
        public static readonly Int3 One = new Int3(1, 1, 1);

        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public int Y;

        /// <summary>
        /// The Z component of the vector.
        /// </summary>
        public int Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Int3"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public Int3(int value)
        {
            X = Y = Z = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int3" /> struct.
        /// </summary>
        /// <param name="x">Initial value for the X component of the vector.</param>
        /// <param name="y">Initial value for the Y component of the vector.</param>
        /// <param name="z">Initial value for the Z component of the vector.</param>
        public Int3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        /// <summary>
        /// Creates an array containing the elements of the vector.
        /// </summary>
        /// <returns>A three-element array containing the components of the vector.</returns>
        public int[] ToArray() => new int[] { X, Y, Z };

        /// <summary>
        /// Performs an explicit conversion from <see cref="Int4" /> to <see cref="Int2" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Int2(Int3 value) => new Int2(value.X, value.Y);

        /// <summary>
        /// Performs an explicit conversion from <see cref="Int3" /> to <see cref="Vector2"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector2(Int3 value) => new Vector2(value.X, value.Y);

        /// <summary>
        /// Performs an explicit conversion from <see cref="Int3" /> to <see cref="Vector3"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector3(Int3 value) => new Vector3(value.X, value.Y, value.Z);

        /// <inheritdoc/>
		public override bool Equals(object obj) => obj is Int3 value && Equals(ref value);

        /// <summary>
        /// Determines whether the specified <see cref="Int3"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Int3"/> to compare with this instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Int3 other) => Equals(ref other);

        /// <summary>
		/// Determines whether the specified <see cref="Int3"/> is equal to this instance.
		/// </summary>
		/// <param name="other">The <see cref="Int3"/> to compare with this instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ref Int3 other)
        {
            return X == other.X
                && Y == other.Y
                && Z == other.Z;
        }

        /// <summary>
        /// Compares two <see cref="Int3"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Int3"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="Int3"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Int3 left, Int3 right) => left.Equals(ref right);

        /// <summary>
        /// Compares two <see cref="Int3"/> objects for inequality.
        /// </summary>
        /// <param name="left">The <see cref="Int3"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="Int3"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Int3 left, Int3 right) => !left.Equals(ref right);

        /// <inheritdoc/>
		public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(Int3)}({X}, {Y}, {Z})";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{nameof(Int3)}({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)})";
        }
    }
}
