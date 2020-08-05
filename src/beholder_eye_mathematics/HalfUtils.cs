namespace beholder_eye_mathematics
{
    // Code adopter from DirectXMath (XMConvertHalfToFloat, XMConvertFloatToHalf)
    internal static class HalfUtils
    {
        public static unsafe float ConvertHalfToFloat(ushort value)
        {
            uint Mantissa = (uint)(value & 0x03FF);
            uint Exponent = (uint)(value & 0x7C00);
            if (Exponent == 0x7C00) // INF/NAN
            {
                Exponent = 0x8f;
            }
            else if (Exponent != 0)  // The value is normalized
            {
                Exponent = ((uint)((int)value >> 10)) & 0x1F;
            }
            else if (Mantissa != 0)     // The value is denormalized
            {
                // Normalize the value in the resulting float
                Exponent = 1;

                do
                {
                    Exponent--;
                    Mantissa <<= 1;
                } while ((Mantissa & 0x0400) == 0);

                Mantissa &= 0x03FF;
            }
            else  // The value is zero
            {
                Exponent = unchecked((uint)-112);
            }

            uint result =
                (((uint)value & 0x8000) << 16)  // Sign
                | ((Exponent + 112) << 23)      // Exponent
                | (Mantissa << 13);             // Mantissa

            return *(float*)(&result);
        }


        public static unsafe ushort ConvertFloatToHalf(float value)
        {
            uint result;

            uint uValue = *(uint*)(&value);
            uint sign = (uValue & 0x80000000U) >> 16;
            uValue = uValue & 0x7FFFFFFFU;      // Hack off the sign

            if (uValue > 0x477FE000U)
            {
                // The number is too large to be represented as a half.  Saturate to infinity.
                if (((uValue & 0x7F800000) == 0x7F800000) && ((uValue & 0x7FFFFF) != 0))
                {
                    result = 0x7FFF; // NAN
                }
                else
                {
                    result = 0x7C00U; // INF
                }
            }
            else if (uValue == 0)
            {
                result = 0;
            }
            else
            {
                if (uValue < 0x38800000U)
                {
                    // The number is too small to be represented as a normalized half.
                    // Convert it to a denormalized value.
                    var shift = (int)(113 - (uValue >> 23));
                    uValue = (0x800000 | (uValue & 0x7FFFFF)) >> shift;
                }
                else
                {
                    // Rebias the exponent to represent the value as a normalized half.
                    uValue += 0xC8000000U;
                }

                result = ((uValue + 0x0FFFU + ((uValue >> 13) & 1)) >> 13) & 0x7FFFU;
            }
            return (ushort)(result | sign);
        }
    }
}
