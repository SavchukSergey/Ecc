using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        public static BigInteger192 DivRemGuess(in BigInteger192 dividend, in BigInteger192 divisor, out BigInteger192 remainder) {
            var divizorLZC = divisor.LeadingZeroCount();

            if (divizorLZC >= BITS_SIZE - 32) {
                return DivRem(in dividend, divisor.LowUInt32, out remainder);
            }
            if (divizorLZC >= BITS_SIZE - 64) {
                return DivRem(in dividend, divisor.LowUInt64, out remainder);
            }
            if (divizorLZC >= BITS_SIZE - 128) {
                return DivRem(in dividend, in divisor.BiLow128, out remainder);
            }

            return new BigInteger192(
                DivRemGuessSingleShot(in dividend, in divisor, out remainder)
            );
        }

        private static ulong DivRemGuessSingleShot(in BigInteger192 dividend, in BigInteger192 divisor, out BigInteger192 remainder) {
            // actual quotient is 64 bit wide
            var divizorLZC = divisor.LeadingZeroCount();
            remainder = dividend;
            var partialDivisor = divisor.ExtractHigh64(divizorLZC) + 1;
            if (partialDivisor != 0) {
                var remainderLZC = remainder.LeadingZeroCount();
                var q128 = remainder.ExtractHigh128(remainderLZC) / partialDivisor;
                var correction = remainderLZC - divizorLZC + 64;
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

        public static BigInteger192 DivRemGuess(in BigInteger192 dividend, in BigInteger128 divisor, out BigInteger128 remainder) {
            var divizorLZC = divisor.LeadingZeroCount();

            if (divizorLZC >= BITS_SIZE - 32) {
                return DivRem(in dividend, divisor.LowUInt32, out remainder);
            }
            if (divizorLZC >= BITS_SIZE - 64) {
                return DivRem(in dividend, divisor.LowUInt64, out remainder);
            }

            var q128 = new BigInteger128(); // actual quotient is 128 bit wide

            var partialDivisor = divisor.ExtractHigh64(divizorLZC) + 1;

            var fullRemainder = dividend.Clone();

            divizorLZC += 64; //pretend to be 192-bit wide

            while (true) {
                var remainderLZC = fullRemainder.LeadingZeroCount();
                if (remainderLZC == divizorLZC) {
                    if (fullRemainder >= divisor) {
                        fullRemainder.AssignSub(divisor);
                        q128.AssignIncrement();
                    }
                    break;
                }
                if (remainderLZC > divizorLZC) {
                    break;
                }

                // pessimistic guess
                var guess = partialDivisor != 0 ?
                    fullRemainder.ExtractHigh128(remainderLZC) / partialDivisor :
                    new BigInteger128(fullRemainder.ExtractHigh64(remainderLZC));
                var correction = remainderLZC - divizorLZC + 64;
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
            return new BigInteger192(q128);
        }

    }
}