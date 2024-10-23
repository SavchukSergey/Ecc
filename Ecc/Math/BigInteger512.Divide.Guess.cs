using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public static BigInteger512 DivRemGuess(in BigInteger512 dividend, in BigInteger512 divisor, out BigInteger512 remainder) {
            var divShiftBits = divisor.LeadingZeroCount();

            if (divShiftBits >= BITS_SIZE - 32) {
                return DivRem32(dividend, divisor.UInt32[0], out remainder);
            }
            if (divShiftBits >= BITS_SIZE - 64) {
                return DivRem64(dividend, divisor.UInt64[0], out remainder);
            }
            if (divShiftBits >= BITS_SIZE - 128) {
                return DivRemGuess(dividend, divisor.Low128, out remainder);
            }
            if (divShiftBits >= BITS_SIZE - 256) {
                return DivRemGuess(dividend, divisor.Low, out remainder);
            }

            var q256 = new BigInteger256();

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divShiftBits);

            var partialDivisor = divisorN.HighUInt64 + 1;

            remainder = dividend;

            while (true) {
                var remainderLZC = remainder.LeadingZeroCount();
                //todo: when approaching to divShiftBits switch to compare-and-subtract strategy
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

                // pessimistic guess
                UInt128 guess = partialDivisor != 0 ? remainder.ExtractHigh128(remainderLZC).UInt128 / partialDivisor : remainder.ExtractHigh64(remainderLZC);
                var correction = remainderLZC - divShiftBits + 64;
                if (correction > 0) {
                    //trim fractional part
                    guess >>= correction;
                    correction = 0;
                }

                var delta = MulLow(divisor, guess);

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

        public static BigInteger512 DivRemGuess(in BigInteger512 dividend, in BigInteger256 divisor, out BigInteger512 remainder) {
            var divShiftBits = divisor.LeadingZeroCount();
            if (divShiftBits >= BITS_SIZE - 32) {
                return DivRem32(dividend, divisor.UInt32[0], out remainder);
            }
            if (divShiftBits >= BITS_SIZE - 64) {
                return DivRem64(dividend, divisor.UInt64[0], out remainder);
            }
            if (divShiftBits >= BITS_SIZE - 128) {
                return DivRemGuess(dividend, divisor.BiLow, out remainder);
            }

            var q = new BigInteger512();

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divShiftBits);
            divShiftBits += BITS_SIZE - BigInteger256.BITS_SIZE;

            var partialDivisor = divisorN.HighUInt64 + 1;

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

                // pessimistic guess
                UInt128 guess = partialDivisor != 0 ? remainder.ExtractHigh128(remainderLZC).UInt128 / partialDivisor : remainder.ExtractHigh64(remainderLZC);
                var correction = remainderLZC - divShiftBits + 64;
                if (correction > 0) {
                    //trim fractional part
                    guess >>= correction;
                    correction = 0;
                }

                // max quotient - 512 bits,
                // 128 bit <= divisor < 256 bits
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

        public static BigInteger512 DivRemGuess(in BigInteger512 dividend, in BigInteger128 divisor, out BigInteger512 remainder) {
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

            var partialDivisor = divisorN.HighUInt64 + 1;

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

                // pessimistic guess
                UInt128 guess = partialDivisor != 0 ? remainder.ExtractHigh128(remainderLZC).UInt128 / partialDivisor : remainder.ExtractHigh64(remainderLZC);
                var correction = remainderLZC - divShiftBits + 64;
                if (correction > 0) {
                    //trim fractional part
                    guess >>= correction;
                    correction = 0;
                }

                // max quotient - 384 bits,
                // 64 bit <= divisor < 128 bits
                var delta = new BigInteger512(divisor * guess);

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