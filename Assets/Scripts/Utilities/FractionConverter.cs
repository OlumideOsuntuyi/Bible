using System.Linq;
using System.Text;
using System;

public static class FractionConverter
{
    public static (int numerator, int denominator) Convert(float value, float tolerance = 1.0e-6f)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
            throw new ArgumentException("Value must be a finite number");

        // Handle negative numbers
        bool isNegative = value < 0;
        value = Math.Abs(value);

        // Handle simple cases
        if (Math.Abs(value) < tolerance) return (0, 1);
        if (Math.Abs(value - Math.Round(value)) < tolerance)
            return (isNegative ? -(int)Math.Round(value) : (int)Math.Round(value), 1);

        // Set initial bounds
        float errorTolerance = tolerance;
        int lowerN = 0;
        int lowerD = 1;
        int upperN = 1;
        int upperD = 1;

        while (true)
        {
            // Middle fraction is (lowerN + upperN) / (lowerD + upperD)
            int middleN = lowerN + upperN;
            int middleD = lowerD + upperD;

            if (middleD * (value + errorTolerance) < middleN)
            {
                // Middle is too big
                upperN = middleN;
                upperD = middleD;
            }
            else if (middleN < middleD * (value - errorTolerance))
            {
                // Middle is too small
                lowerN = middleN;
                lowerD = middleD;
            }
            else
            {
                // Found a good approximation
                int numerator = middleN;
                int denominator = middleD;

                // Reduce the fraction
                int gcd = GCD(numerator, denominator);
                numerator /= gcd;
                denominator /= gcd;

                return (isNegative ? -numerator : numerator, denominator);
            }

            // Check for overflow
            if (middleN > int.MaxValue / 2 || middleD > int.MaxValue / 2)
            {
                // Reduce fractions to avoid overflow
                int gcd1 = GCD(upperN, upperD);
                upperN /= gcd1;
                upperD /= gcd1;

                int gcd2 = GCD(lowerN, lowerD);
                lowerN /= gcd2;
                lowerD /= gcd2;
            }
        }
    }

    // Helper method to calculate the greatest common divisor (GCD)
    private static int GCD(int a, int b)
    {
        a = Math.Abs(a);
        b = Math.Abs(b);

        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }
}