namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static void DivRem(in BigInteger256 dividend, in BigInteger192 divisor, out BigInteger256 quotient, out BigInteger192 remainder) {
            if (divisor.HighUInt64 == 0) {
                remainder = new BigInteger192();
                DivRem(in dividend, in divisor.BiLow128, out quotient, out remainder.BiLow128);
                return;
            }
            if (dividend.HighUInt64 == 0) {
                quotient = new BigInteger256();
                BigInteger192.DivRem(in dividend.BiLow192, in divisor, out quotient.BiLow192, out remainder);
                return;
            }

            var divisorLZC = divisor.LeadingZeroCount();

            var q = new BigInteger128(); // actual quotient is 128 bit wide

            var partialDivisor = divisor.ExtractHigh64(divisorLZC) + 1;

            var fullRemainder = dividend.Clone();

            divisorLZC += 64; //pretend to be 256-bit wide

            while (true) {
                var remainderLZC = fullRemainder.LeadingZeroCount();
                if (remainderLZC == divisorLZC) {
                    if (fullRemainder >= divisor) {
                        fullRemainder.AssignSub(divisor);
                        q.AssignIncrement();
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

                var delta = BigInteger192.MulLow256(divisor, guess);

                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guess <<= -correction;
                }
                fullRemainder.AssignSub(delta);
                q.AssignAdd(guess);
            }

            remainder = fullRemainder.BiLow192;
            quotient = new BigInteger256(q);
        }

    }
}