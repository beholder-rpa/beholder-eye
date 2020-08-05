namespace beholder_eye_win.DXGI
{
    public partial struct Rational
    {
        /// <summary>
        /// Initialize instance of <see cref="Rational"/> struct.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public Rational(int numerator, int denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public override string ToString() => $"Numerator: {Numerator}, Denominator: {Denominator}";
    }
}
