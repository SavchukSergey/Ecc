using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public static BigInteger512 DivRemGuess(in BigInteger512 dividend, in BigInteger512 divisor, out BigInteger512 remainder) {
            var divShiftBits = divisor.LeadingZeroCount();

            if (divShiftBits >= BITS_SIZE - 32) {
                var res = DivRem(in dividend, divisor.UInt32[0], out uint remainder32);
                remainder = new BigInteger512(remainder32);
                return res;
            }
            if (divShiftBits >= BITS_SIZE - 64) {
                var res = DivRem(in dividend, divisor.UInt64[0], out ulong remainder64);
                remainder = new BigInteger512(remainder64);
                return res;
            }
            if (divShiftBits >= BITS_SIZE - 128) {
                var res = DivRemGuess(in dividend, in divisor.Low128, out BigInteger128 remainder128);
                remainder = new BigInteger512(remainder128);
                return res;
            }
            if (divShiftBits >= BITS_SIZE - 256) {
                var res = DivRemGuess(in dividend, divisor.Low, out BigInteger256 remainder256);
                remainder = new BigInteger512(remainder256);
                return res;
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
                var guess = partialDivisor != 0 ?
                    remainder.ExtractHigh128(remainderLZC) / partialDivisor :
                    new BigInteger128(remainder.ExtractHigh64(remainderLZC));
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger512 DivRemGuess(in BigInteger512 dividend, in BigInteger256 divisor, out BigInteger512 remainder) {
            var res = DivRemGuess(in dividend, in divisor, out BigInteger256 remainder256);
            remainder = new BigInteger512(remainder256);
            return res;
        }

        public static BigInteger512 DivRemGuess(in BigInteger512 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            var divShiftBits = divisor.LeadingZeroCount();
            if (divShiftBits >= BITS_SIZE - 32) {
                var res = DivRem(dividend, divisor.UInt32[0], out uint remainder32);
                remainder = new BigInteger256(remainder32);
                return res;
            }
            if (divShiftBits >= BITS_SIZE - 64) {
                var res = DivRem(dividend, divisor.UInt64[0], out ulong remainder64);
                remainder = new BigInteger256(remainder64);
                return res;
            }
            if (divShiftBits >= BITS_SIZE - 128) {
                var res = DivRemGuess(dividend, divisor.BiLow, out BigInteger128 remainder128);
                remainder = new BigInteger256(remainder128);
                return res;
            }

            var q = new BigInteger512();

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divShiftBits);
            divShiftBits += BITS_SIZE - BigInteger256.BITS_SIZE;

            var partialDivisor = divisorN.HighUInt64 + 1;

            var fullRemainder = dividend.Clone();

            while (true) {
                var remainderLZC = fullRemainder.LeadingZeroCount();
                if (remainderLZC == divShiftBits) {
                    if (fullRemainder >= divisor) {
                        fullRemainder.AssignSub(divisor);
                        q.AssignIncrement();
                    }
                    break;
                }
                if (remainderLZC > divShiftBits) {
                    break;
                }

                // pessimistic guess
                var guess = partialDivisor != 0 ?
                    fullRemainder.ExtractHigh128(remainderLZC) / partialDivisor :
                    new BigInteger128(fullRemainder.ExtractHigh64(remainderLZC));
                var correction = remainderLZC - divShiftBits + 64;
                if (correction > 0) {
                    //trim fractional part
                    guess >>= correction;
                    correction = 0;
                }

                // max quotient - 512 bits,
                // 128 bit <= divisor < 256 bits
                var delta = new BigInteger512(divisor * guess); //todo:

                var guessQ = new BigInteger512(guess);
                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guessQ.AssignLeftShift(-correction);
                }
                fullRemainder.AssignSub(delta);
                q.AssignAdd(guessQ);
            }

            remainder = fullRemainder.Low;
            return q;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger512 DivRemGuess(in BigInteger512 dividend, in BigInteger128 divisor, out BigInteger512 remainder) {
            var res = DivRemGuess(in dividend, in divisor, out BigInteger128 remainder128);
            remainder = new BigInteger512(remainder128);
            return res;
        }

        public static BigInteger512 DivRemGuess(in BigInteger512 dividend, in BigInteger128 divisor, out BigInteger128 remainder) {
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

            var q = new BigInteger512();

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divShiftBits);
            divShiftBits += BITS_SIZE - BigInteger128.BITS_SIZE;

            var partialDivisor = divisorN.HighUInt64 + 1;

            var fullRemainder = dividend.Clone();

            while (true) {
                var remainderLZC = fullRemainder.LeadingZeroCount();
                if (remainderLZC == divShiftBits) {
                    if (fullRemainder >= divisor) {
                        fullRemainder.AssignSub(divisor);
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

                // max quotient - 384 bits,
                // 64 bit <= divisor < 128 bits
                var delta = new BigInteger512(divisor * guess);

                var guessQ = new BigInteger512(guess);
                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guessQ.AssignLeftShift(-correction);
                }
                fullRemainder.AssignSub(delta);
                q.AssignAdd(guessQ);
            }

            remainder = fullRemainder.BiLow128;

            return q;
        }
    }
}