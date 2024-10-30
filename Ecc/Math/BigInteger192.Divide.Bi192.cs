namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        public static void DivRem(in BigInteger192 dividend, in BigInteger192 divisor, out BigInteger192 quotient, out BigInteger192 remainder) {
            if (divisor.HighUInt64 == 0) {
                remainder = new BigInteger192();
                DivRem(in dividend, in divisor.BiLow128, out quotient, out remainder.BiLow128);
                return;
            }

            //todo: check is dividendLZC - divisorLZC < 64 then call SingleShot
            // actual quotient is 64 bit wide
            var divisorLZC = divisor.LeadingZeroCount();
            remainder = dividend;
            var partialDivisor = divisor.ExtractHigh64(divisorLZC) + 1;
            if (partialDivisor != 0) {
                var remainderLZC = remainder.LeadingZeroCount();
                var q128 = remainder.ExtractHigh128(remainderLZC) / partialDivisor;
                var correction = remainderLZC - divisorLZC + 64;
                if (correction > 0) {
                    //trim fractional part
                    q128 >>= correction;
                }
                var q64 = q128.LowUInt64;

                MulLow192(divisor, q64, out var delta);
                remainder.AssignSub(delta);

                if (remainder >= divisor) {
                    remainder.AssignSub(divisor);
                    q64++;

                    if (remainder >= divisor) {
                        remainder.AssignSub(divisor);
                        q64++;
                    }
                }
                quotient = new BigInteger192(q64);
            } else {
                //this can happen only if divisor starts with 64 ones, quotient will be either 0 or 1
                if (remainder >= divisor) {
                    remainder.AssignSub(divisor);
                    quotient = new BigInteger192(1);
                } else {
                    quotient = new BigInteger192(0);
                }
            }
        }

    }
}