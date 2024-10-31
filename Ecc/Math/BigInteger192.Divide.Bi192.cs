namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        public static void DivRem(in BigInteger192 dividend, in BigInteger192 divisor, out BigInteger192 quotient, out BigInteger192 remainder) {
            if (divisor.HighUInt64 == 0) {
                remainder = new BigInteger192();
                DivRem(in dividend, in divisor.BiLow128, out quotient, out remainder.BiLow128);
                return;
            }

            // actual quotient is 64 bit wide
            var divisorLZC = divisor.LeadingZeroCount();
            remainder = dividend;
            var remainderLZC = remainder.LeadingZeroCount();
            var correction = remainderLZC - divisorLZC + 64;
            var partialDivisor = divisor.ExtractHigh64(divisorLZC) + 1;
            ulong q64;
            if (partialDivisor != 0) {
                var rem128 = remainder.ExtractHigh128(remainderLZC);
                BigInteger128.DivRem(in rem128, partialDivisor, out var q128, out var _);
                if (correction > 0) {
                    //trim fractional part
                    q128 >>= correction;
                }
                q64 = q128.LowUInt64;
            } else {
                //this can happen only if divisor starts with 64 ones

                q64 = remainder.ExtractHigh64(remainderLZC);
                if (correction > 0) {
                    //trim fractional part
                    q64 = correction < 64 ? q64 >> correction : 0;
                }
            }
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
        }

    }
}