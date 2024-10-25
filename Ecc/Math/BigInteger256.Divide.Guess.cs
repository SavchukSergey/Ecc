using System;
namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger256 DivRemGuess(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            var divizorLZC = divisor.LeadingZeroCount();

            if (divizorLZC >= BITS_SIZE - 32) {
                return DivRem(dividend, divisor.LowUInt32, out remainder);
            }
            if (divizorLZC >= BITS_SIZE - 64) {
                return DivRem(dividend, divisor.LowUInt64, out remainder);
            }
            if (divizorLZC >= BITS_SIZE - 128) {
                return DivRem(dividend, in divisor.BiLow, out remainder);
            }
            if (divizorLZC >= BITS_SIZE - 192) {
                return DivRem(dividend, in divisor.BiLow192, out remainder);
            }

            //todo: move logic below to generic DivRem
            // actual quotient is 64 bit wide
            remainder = dividend;
            var partialDivisor = divisor.ExtractHigh64(divizorLZC) + 1;
            if (partialDivisor != 0) {
                var remainderLZC = remainder.LeadingZeroCount();
                UInt128 q128 = remainder.ExtractHigh128(remainderLZC).UInt128 / partialDivisor;
                var correction = remainderLZC - divizorLZC + 64;
                if (correction > 0) {
                    //trim fractional part
                    q128 >>= correction;
                }
                var q64 = (ulong)q128;

                var delta = MulLow(divisor, q64);
                remainder.AssignSub(delta);

                if (remainder >= divisor) {
                    remainder.AssignSub(divisor);
                    q64++;

                    if (remainder >= divisor) {
                        remainder.AssignSub(divisor);
                        q64++;
                    }
                }
                return new BigInteger256(q64);
            } else {
                //this can happen only if divisor starts with 64 ones, quotient will be either 0 or 1
                if (remainder >= divisor) {
                    remainder.AssignSub(divisor);
                    return new BigInteger256(1);
                }

                return new BigInteger256(0);
            }
        }

        public static BigInteger256 DivRemGuess(in BigInteger256 dividend, in BigInteger192 divisor, out BigInteger192 remainder) {
            var divizorLZC = divisor.LeadingZeroCount();

            if (divizorLZC >= BITS_SIZE - 32) {
                return DivRem(dividend, divisor.LowUInt32, out remainder);
            }
            if (divizorLZC >= BITS_SIZE - 64) {
                return DivRem(dividend, divisor.LowUInt64, out remainder);
            }
            if (divizorLZC >= BITS_SIZE - 128) {
                return DivRem(dividend, divisor.BiLow128, out remainder);
            }

            var q128 = new BigInteger128(); // actual quotient is 128 bit wide

            var partialDivisor = divisor.ExtractHigh64(divizorLZC) + 1;

            var fullRemainder = dividend.Clone();

            divizorLZC += 64; //pretend to be 256-bit wide

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
                UInt128 guess = partialDivisor != 0 ? fullRemainder.ExtractHigh128(remainderLZC).UInt128 / partialDivisor : fullRemainder.ExtractHigh64(remainderLZC);
                var correction = remainderLZC - divizorLZC + 64;
                if (correction > 0) {
                    //trim fractional part
                    guess >>= correction;
                    correction = 0;
                }

                var delta = MulLow(new BigInteger256(divisor), guess); //todo:

                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guess <<= -correction;
                }
                fullRemainder.AssignSub(delta);
                q128.AssignAdd(guess);
            }

            remainder = fullRemainder.BiLow192;
            return new BigInteger256(q128);
        }

        public static BigInteger256 DivRemGuess(in BigInteger256 dividend, in BigInteger128 divisor, out BigInteger128 remainder) {
            var divShiftBits = divisor.LeadingZeroCount();
            if (divShiftBits >= BITS_SIZE - 32) {
                var res = DivRem(dividend, divisor.UInt32[0], out uint remainder32);
                remainder = new BigInteger128(remainder32);
                return res;
            }
            if (divShiftBits >= BITS_SIZE - 64) {
                var res = DivRem(dividend, divisor.UInt64[0], out ulong remainder64);
                remainder = new BigInteger128(remainder64);
                return res;
            }

            var q = new BigInteger256();

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divShiftBits);
            divShiftBits += BITS_SIZE - BigInteger128.BITS_SIZE;

            var partialDivisor = divisorN.HighUInt64 + 1;

            var fullRemainder = dividend;

            while (true) {
                var remainderLZC = fullRemainder.LeadingZeroCount();
                if (remainderLZC == divShiftBits) {
                    if (fullRemainder.BiLow >= divisor) {
                        fullRemainder.BiLow.AssignSub(divisor);
                        q.AssignIncrement();
                    }
                    break;
                }
                if (remainderLZC > divShiftBits) {
                    break;
                }

                // pessimistic guess
                UInt128 guess = partialDivisor != 0 ? fullRemainder.ExtractHigh128(remainderLZC).UInt128 / partialDivisor : fullRemainder.ExtractHigh64(remainderLZC);
                var correction = remainderLZC - divShiftBits + 64;
                if (correction > 0) {
                    //trim fractional part
                    guess >>= correction;
                    correction = 0;
                }

                // max quotient - 128 bits,
                // 64 bit <= divisor < 128 bits
                // guess - 128 bits
                var delta = divisor * guess;

                var guessQ = new BigInteger256(guess);
                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guessQ.AssignLeftShift(-correction);
                }
                fullRemainder.AssignSub(delta);
                q.AssignAdd(guessQ);
            }

            remainder = fullRemainder.BiLow;
            return q;
        }

    }
}