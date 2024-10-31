namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        public static void DivRem(in BigInteger192 dividend, in BigInteger128 divisor, out BigInteger192 quotient, out BigInteger128 remainder) {
            if (divisor.HighUInt64 == 0) {
                remainder = new BigInteger128();
                DivRem(in dividend, divisor.LowUInt64, out quotient, out remainder.LowUInt64);
                return;
            }
            if (dividend.HighUInt64 == 0) {
                quotient = new BigInteger192();
                BigInteger128.DivRem(in dividend.BiLow128, in divisor, out quotient.BiLow128, out remainder);
                return;
            }
            var divisorLZC = divisor.LeadingZeroCount();

            var quotientSmall = new BigInteger128(); // actual quotient is 128 bit wide

            var partialDivisor = divisor.ExtractHigh64(divisorLZC) + 1;

            var fullRemainder = dividend.Clone();

            divisorLZC += 64; //pretend to be 192-bit wide

            while (true) {
                var remainderLZC = fullRemainder.LeadingZeroCount();
                if (remainderLZC == divisorLZC) {
                    if (fullRemainder >= divisor) {
                        fullRemainder.AssignSub(divisor);
                        quotientSmall.AssignIncrement();
                    }
                    break;
                }
                if (remainderLZC > divisorLZC) {
                    break;
                }

                // pessimistic guess
                var guess = partialDivisor != 0 ?
                    fullRemainder.ExtractHigh128(remainderLZC) / partialDivisor :
                    new BigInteger128(fullRemainder.ExtractHigh64(remainderLZC));
                var correction = remainderLZC - divisorLZC + 64;
                if (correction > 0) {
                    //trim fractional part
                    guess >>= correction;
                    correction = 0;
                }

                BigInteger128.MulLow192(divisor, guess, out var delta);

                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guess <<= -correction;
                }
                fullRemainder.AssignSub(delta);
                quotientSmall.AssignAdd(guess);
            }

            remainder = fullRemainder.BiLow128;
            quotient = new BigInteger192(quotientSmall);
        }
    }
}