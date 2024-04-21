using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public static BigInteger512 DivRemGuess(in BigInteger512 dividend, in BigInteger256 divisor, out BigInteger512 remainder) {
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

            var q256 = new BigInteger256();

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divShiftBits);
            divShiftBits += BITS_SIZE - BigInteger256.BITS_SIZE;

            var divPart64 = (UInt128)divisorN.HighUInt64;

            remainder = dividend;

            while (true) {
                var remainderLZC = remainder.LeadingZeroCount();
                if (remainderLZC == divShiftBits) {
                    if (remainder >= divisor) {
                        remainder.AssignSub(divisor);
                        q256.AssignIncrement();
                    }
                    break;
                }
                if (remainderLZC > divShiftBits) {
                    break;
                }
                var remainderAdjusted = remainder << remainderLZC;

                var remPart128 = remainderAdjusted.HighUInt128;

                // pessimistic guess
                UInt128 guess = divPart64 != ulong.MaxValue ? remPart128 / (divPart64 + 1) : remPart128 >> 64;
                var correction = remainderLZC - divShiftBits + 64;
                if (correction > 0) {
                    //trim fractional part
                    guess >>= correction;
                    correction = 0;
                }

                // max quotient - 256 bits,
                // 128 bit <= divisor < 256 bits
                var delta = BigInteger256.MulLow(divisor, guess);

                var guessQ = new BigInteger256(guess);
                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guessQ.AssignLeftShift(-correction);
                }
                remainder.AssignSub(delta);
                q256.AssignAdd(guessQ);
            }

            return new BigInteger512(q256);
        }

        public static BigInteger512 DivRemGuess128(in BigInteger512 dividend, in BigInteger128 divisor, out BigInteger512 remainder) {
            var divShiftBits = divisor.LeadingZeroCount();
            if (divShiftBits >= BITS_SIZE - 32) {
                return DivRem32(dividend, divisor.UInt32[0], out remainder);
            }
            if (divShiftBits >= BITS_SIZE - 64) {
                return DivRem64(dividend, divisor.UInt64[0], out remainder);
            }

            var q = new BigInteger512();

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divShiftBits);
            divShiftBits += BITS_SIZE - BigInteger128.BITS_SIZE;

            var divPart64 = (UInt128)divisorN.HighUInt64;

            remainder = dividend;

            while (true) {
                var remainderLZC = remainder.LeadingZeroCount();
                if (remainderLZC == divShiftBits) {
                    if (remainder >= divisor) {
                        remainder.AssignSub(divisor);
                        q.AssignIncrement();
                    }
                    break;
                }
                if (remainderLZC > divShiftBits) {
                    break;
                }
                var remainderAdjusted = remainder << remainderLZC;

                var remPart128 = remainderAdjusted.HighUInt128;

                // pessimistic guess
                UInt128 guess = divPart64 != ulong.MaxValue ? remPart128 / (divPart64 + 1) : remPart128 >> 64;
                var correction = remainderLZC - divShiftBits + 64;
                if (correction > 0) {
                    //trim fractional part
                    guess >>= correction;
                    correction = 0;
                }

                // max quotient - 384 bits,
                // 64 bit <= divisor < 128 bits
                var delta = divisor * guess;

                var guessQ = new BigInteger512(guess);
                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guessQ.AssignLeftShift(-correction);
                }
                remainder.AssignSub(delta);
                q.AssignAdd(guessQ);
            }

            return q;
        }
    }
}