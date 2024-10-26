namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor">Must be >= 2^128</param>
        /// <param name="remainder"></param>
        /// <returns></returns>
        private static ulong DivRemGuessSingleShot(in BigInteger192 dividend, in BigInteger192 divisor, out BigInteger192 remainder) {
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

                var delta = MulLow192(divisor, q64);
                remainder.AssignSub(delta);

                if (remainder >= divisor) {
                    remainder.AssignSub(divisor);
                    q64++;

                    if (remainder >= divisor) {
                        remainder.AssignSub(divisor);
                        q64++;
                    }
                }
                return q64;
            } else {
                //this can happen only if divisor starts with 64 ones, quotient will be either 0 or 1
                if (remainder >= divisor) {
                    remainder.AssignSub(divisor);
                    return 1;
                }

                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor">Must be >= 2^64</param>
        /// <param name="remainder"></param>
        /// <returns></returns>
        private static BigInteger128 DivRemGuess(in BigInteger192 dividend, in BigInteger128 divisor, out BigInteger128 remainder) {
            var divisorLZC = divisor.LeadingZeroCount();

            var q128 = new BigInteger128(); // actual quotient is 128 bit wide

            var partialDivisor = divisor.ExtractHigh64(divisorLZC) + 1;

            var fullRemainder = dividend.Clone();

            divisorLZC += 64; //pretend to be 192-bit wide

            while (true) {
                var remainderLZC = fullRemainder.LeadingZeroCount();
                if (remainderLZC == divisorLZC) {
                    if (fullRemainder >= divisor) {
                        fullRemainder.AssignSub(divisor);
                        q128.AssignIncrement();
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

                var delta = BigInteger128.MulLow192(divisor, guess);

                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guess <<= -correction;
                }
                fullRemainder.AssignSub(delta);
                q128.AssignAdd(guess);
            }

            remainder = fullRemainder.BiLow128;
            return q128;
        }

    }
}