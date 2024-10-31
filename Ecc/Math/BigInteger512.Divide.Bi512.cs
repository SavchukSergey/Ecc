using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger512 DivRem(in BigInteger512 dividend, in BigInteger512 divisor, out BigInteger512 remainder) {
            DivRem(in dividend, in divisor, out var quotient, out remainder);
            return quotient;
        }

        public static void DivRem(in BigInteger512 dividend, in BigInteger512 divisor, out BigInteger512 quotient, out BigInteger512 remainder) {
            if (divisor.BiHigh256.IsZero) {
                remainder = new BigInteger512();
                DivRem(in dividend, divisor.BiLow256, out quotient, out remainder.BiLow256);
                return;
            }
            var divisorLZC = divisor.LeadingZeroCount();

            var q256 = new BigInteger256();

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divisorLZC);

            var partialDivisor = divisorN.HighUInt64 + 1;

            remainder = dividend;

            while (true) {
                var remainderLZC = remainder.LeadingZeroCount();
                //todo: when approaching to divShiftBits switch to compare-and-subtract strategy
                if (remainderLZC == divisorLZC) {
                    if (remainder >= divisor) {
                        remainder.AssignSub(divisor);
                        q256.AssignIncrement();
                    }
                    break;
                }
                if (remainderLZC > divisorLZC) {
                    break;
                }

                // pessimistic guess
                var guess = partialDivisor != 0 ?
                    remainder.ExtractHigh128(remainderLZC) / partialDivisor :
                    new BigInteger128(remainder.ExtractHigh64(remainderLZC));
                var correction = remainderLZC - divisorLZC + 64;
                if (correction > 0) {
                    //trim fractional part
                    guess >>= correction;
                    correction = 0;
                }

                MulLow512(divisor, guess, out var delta);

                var guessQ = new BigInteger256(guess);
                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guessQ.AssignLeftShift(-correction);
                }
                remainder.AssignSub(delta);
                q256.AssignAdd(guessQ);
            }

            quotient = new BigInteger512(q256);
        }

    }
}