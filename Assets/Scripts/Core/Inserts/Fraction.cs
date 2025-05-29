using System;

namespace Core.Module
{
    public readonly struct Fraction
    {
        public readonly int numerator;
        public readonly int denominator;

        public Fraction(float numeric)
        {
            var frac = FractionConverter.Convert(numeric);
            numerator = frac.numerator;
            denominator = frac.denominator;
        }

        public Fraction(int numerator=0, int denominator=1)
        {
            this.numerator = numerator;
            this.denominator = denominator;
        }


        public static implicit operator Value (Fraction number)
        {
            return new Value(number, ValueType.Fraction);
        }

        public static implicit operator float(Fraction number)
        {
            return number.numerator / (float)number.denominator;
        }

        public static implicit operator Fraction(float number)
        {
            var frac = FractionConverter.Convert(number);
            return new Fraction(frac.numerator, frac.denominator);
        }

        public static implicit operator Fraction(string str)
        {
            string[] param = str.Split('/');
            _ = Int32.TryParse(param[0], out int num);
            _ = Int32.TryParse(param[1], out int denom);
            return new Fraction(num, denom);
        }

        public override string ToString()
        {
            return $"{numerator}/{denominator}";
        }
    }
}