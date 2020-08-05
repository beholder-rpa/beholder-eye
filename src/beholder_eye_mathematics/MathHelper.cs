namespace beholder_eye_mathematics
{
    using System;

    /// <summary>
    /// Defines a math helper class.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// The value for which all absolute numbers smaller than are considered equal to zero.
        /// </summary>
        public const float ZeroTolerance = 1e-6f; // Value a 8x higher than 1.19209290E-07F

        /// <summary>
        /// Represents the mathematical constant e.
        /// </summary>
        public const float E = 2.71828175f;

        /// <summary>
        /// Represents the log base two of e.
        /// </summary>
        public const float Log2E = 1.442695f;

        /// <summary>
        /// Represents the log base ten of e.
        /// </summary>
        public const float Log10E = 0.4342945f;

        /// <summary>
        /// Represents the value of pi.
        /// </summary>
        public const float Pi = (float)Math.PI;

        /// <summary>
        /// Represents the value of pi times two.
        /// </summary>
        public const float TwoPi = (float)(2 * Math.PI);

        /// <summary>
        /// Represents the value of pi divided by two.
        /// </summary>
        public const float PiOver2 = (float)(Math.PI / 2);

        /// <summary>
        /// Represents the value of pi divided by four.
        /// </summary>
        public const float PiOver4 = (float)(Math.PI / 4);

        /// <summary>
        /// Checks if a - b are almost equals within a float epsilon.
        /// </summary>
        /// <param name="a">The left value to compare.</param>
        /// <param name="b">The right value to compare.</param>
        /// <param name="epsilon">Epsilon value</param>
        /// <returns><c>true</c> if a almost equal to b within a float epsilon, <c>false</c> otherwise</returns>
        public static bool WithinEpsilon(float a, float b, float epsilon)
        {
            float diff = a - b;
            return (-epsilon <= diff) && (diff <= epsilon);
        }

        /// <summary>
        /// Checks if a and b are almost equals, taking into account the magnitude of floating point numbers (unlike <see cref="WithinEpsilon"/> method). See Remarks.
        /// See remarks.
        /// </summary>
        /// <param name="a">The left value to compare.</param>
        /// <param name="b">The right value to compare.</param>
        /// <returns><c>true</c> if a almost equal to b, <c>false</c> otherwise</returns>
        /// <remarks>
        /// The code is using the technique described by Bruce Dawson in 
        /// <a href="http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/">Comparing Floating point numbers 2012 edition</a>. 
        /// </remarks>
        public unsafe static bool NearEqual(float a, float b)
        {
            // Check if the numbers are really close -- needed
            // when comparing numbers near zero.
            if (IsZero(a - b))
                return true;

            // Original from Bruce Dawson: http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
            int aInt = *(int*)&a;
            int bInt = *(int*)&b;

            // Different signs means they do not match.
            if ((aInt < 0) != (bInt < 0))
                return false;

            // Find the difference in ULPs.
            int ulp = Math.Abs(aInt - bInt);

            // Choose of maxUlp = 4
            // according to http://code.google.com/p/googletest/source/browse/trunk/include/gtest/internal/gtest-internal.h
            const int maxUlp = 4;
            return ulp <= maxUlp;
        }

        /// <summary>
        /// Determines whether the specified value is close to zero (0.0f).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns><c>true</c> if the specified value is close to zero (0.0f); otherwise, <c>false</c>.</returns>
        public static bool IsZero(float a) => Math.Abs(a) < ZeroTolerance;

        /// <summary>
        /// Determines whether the specified value is close to one (1.0f).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns><c>true</c> if the specified value is close to one (1.0f); otherwise, <c>false</c>.</returns>
        public static bool IsOne(float a) => IsZero(a - 1.0f);

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The converted value.</returns>
        public static float ToDegrees(float radians) => radians * (180.0f / Pi);

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degree">Converts degrees to radians.</param>
        /// <returns>The converted value.</returns>
        public static float ToRadians(float degree) => degree * (Pi / 180.0f);

        /// <summary>
        /// Clamps the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>The result of clamping a value between min and max</returns>
        public static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>
        /// Clamps the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>The result of clamping a value between min and max</returns>
        public static int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="value1">Source value1.</param>
        /// <param name="value2">Source value2.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns>The result of linear interpolation of values based on the amount.</returns>
        public static float Lerp(float value1, float value2, float amount) => value1 + ((value2 - value1) * amount);

        /// <summary>
        /// Calculates the absolute value of the difference of two values.
        /// </summary>
        /// <param name="value1">Source value1.</param>
        /// <param name="value2">Source value2.</param>
        /// <returns>The distance value.</returns>
        public static float Distance(float value1, float value2) => Math.Abs(value1 - value2);
    }
}
