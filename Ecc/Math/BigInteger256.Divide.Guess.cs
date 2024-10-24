using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger256 DivRemGuess(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            var divShiftBits = divisor.LeadingZeroCount();

            if (divShiftBits >= BITS_SIZE - 32) {
                return DivRem32(dividend, divisor.UInt32[0], out remainder);
            }
            if (divShiftBits >= BITS_SIZE - 64) {
                return DivRem64(dividend, divisor.UInt64[0], out remainder);
            }
            if (divShiftBits >= BITS_SIZE - 128) {
                return DivRemGuess128(dividend, divisor.BiLow, out remainder);
            }

            var q128 = new BigInteger128(); // actual quotient is 128 bit wide

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divShiftBits);

            var partialDivisor = divisorN.HighUInt64 + 1;

            remainder = dividend;

            //todo: it performs better with division by byte rather then u64

            while (true) {
                var remainderLZC = remainder.LeadingZeroCount();
                if (remainderLZC == divShiftBits) {
                    if (remainder >= divisor) {
                        remainder.AssignSub(divisor);
                        q128.AssignIncrement();
                    }
                    break;
                }
                if (remainderLZC > divShiftBits) {
                    break;
                }

                // pessimistic guess
                UInt128 guess = partialDivisor != 0 ? remainder.ExtractHigh128(remainderLZC).UInt128 / partialDivisor : remainder.ExtractHigh64(remainderLZC);
                var correction = remainderLZC - divShiftBits + 64;
                if (correction > 0) {
                    //trim fractional part
                    guess >>= correction;
                    correction = 0;
                }

                var delta = MulLow(divisor, guess);

                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guess <<= -correction;
                }
                remainder.AssignSub(delta);
                q128.AssignAdd(guess);
            }

            return new BigInteger256(q128);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRemGuess128(in BigInteger256 dividend, in BigInteger128 divisor, out BigInteger256 remainder) {
            var res = DivRemGuess128(in dividend, in divisor, out BigInteger128 remainder128);
            remainder = new BigInteger256(remainder128);
            return res;
        }

        public static BigInteger256 DivRemGuess128(in BigInteger256 dividend, in BigInteger128 divisor, out BigInteger128 remainder) {
            var divShiftBits = divisor.LeadingZeroCount();
            if (divShiftBits >= BITS_SIZE - 32) {
                var res = DivRem32(dividend, divisor.UInt32[0], out uint remainder32);
                remainder = new BigInteger128(remainder32);
                return res;
            }
            if (divShiftBits >= BITS_SIZE - 64) {
                var res = DivRem64(dividend, divisor.UInt64[0], out ulong remainder64);
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