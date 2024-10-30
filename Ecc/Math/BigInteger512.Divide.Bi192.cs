namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public static void DivRem(in BigInteger512 dividend, in BigInteger192 divisor, out BigInteger512 quotient, out BigInteger192 remainder) {
            if (divisor.HighUInt64 == 0) {
                remainder = new BigInteger192();
                DivRem(in dividend, in divisor.BiLow128, out quotient, out remainder.BiLow128);
                return;
            }
            if (dividend.BiHigh256.IsZero) {
                quotient = new BigInteger512();
                BigInteger256.DivRem(in dividend.BiLow256, in divisor, out quotient.BiLow256, out remainder);
                return;
            }
            var divisorLZC = divisor.LeadingZeroCount();
            //divisor at least 129-bit wide, quotient is 384 bits at most

            var q = new BigInteger384();

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divisorLZC);
            divisorLZC += BITS_SIZE - BigInteger192.BITS_SIZE;

            var partialDivisor = divisorN.HighUInt64 + 1;

            var fullRemainder = dividend.Clone();

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

                // max quotient - 512 bits,
                // 128 bit <= divisor < 256 bits
                var delta = new BigInteger512(new BigInteger256(divisor) * guess); //todo:

                var guessQ = new BigInteger512(guess);
                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guessQ.AssignLeftShift(-correction);
                }
                fullRemainder.AssignSub(delta);
                q.AssignAdd(guessQ.BiLow384); //todo:
            }

            remainder = fullRemainder.BiLow192;
            quotient = new BigInteger512(q);
        }
        
    }
}