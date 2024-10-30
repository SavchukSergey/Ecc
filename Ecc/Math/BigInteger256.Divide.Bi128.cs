namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static void DivRem(in BigInteger256 dividend, in BigInteger128 divisor, out BigInteger256 quotient, out BigInteger128 remainder) {
            if (divisor.HighUInt64 == 0) {
                remainder = new BigInteger128();
                DivRem(in dividend, divisor.LowUInt64, out quotient, out remainder.LowUInt64);
                return;
            }
            if (dividend.HighUInt64 == 0) {
                quotient = new BigInteger256();
                BigInteger192.DivRem(in dividend.BiLow192, in divisor, out quotient.BiLow192, out remainder);
                return;
            }

            var divisorLZC = divisor.LeadingZeroCount();

            var q = new BigInteger192();

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divisorLZC);
            divisorLZC += BITS_SIZE - BigInteger128.BITS_SIZE;

            var partialDivisor = divisorN.HighUInt64 + 1;

            var fullRemainder = dividend;

            while (true) {
                var remainderLZC = fullRemainder.LeadingZeroCount();
                if (remainderLZC == divisorLZC) {
                    if (fullRemainder.BiLow128 >= divisor) {
                        fullRemainder.BiLow128.AssignSub(divisor);
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

                // max quotient - 128 bits,
                // 64 bit <= divisor < 128 bits
                // guess - 128 bits
                BigInteger128.Mul(in divisor, in guess, out var delta);

                var guessQ = new BigInteger192(guess);
                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guessQ.AssignLeftShift(-correction);
                }
                fullRemainder.AssignSub(delta);
                q.AssignAdd(guessQ);
            }

            remainder = fullRemainder.BiLow128;

            quotient = new BigInteger256(q);
        }

    }
}