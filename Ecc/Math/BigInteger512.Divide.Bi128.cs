using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger512 DivRem(in BigInteger512 dividend, in BigInteger128 divisor, out BigInteger128 remainder) {
            DivRem(in dividend, in divisor, out var quotient, out remainder);
            return quotient;
        }

        public static void DivRem(in BigInteger512 dividend, in BigInteger128 divisor, out BigInteger512 quotient, out BigInteger128 remainder) {
            if (divisor.HighUInt64 == 0) {
                remainder = new BigInteger128();
                DivRem(in dividend, divisor.LowUInt64, out quotient, out remainder.LowUInt64);
                return;
            }
            if (dividend.BiHigh256.IsZero) {
                quotient = new BigInteger512();
                BigInteger256.DivRem(in dividend.BiLow256, in divisor, out quotient.BiLow256, out remainder);
                return;
            }
            var divisorLZC = divisor.LeadingZeroCount();

            quotient = new BigInteger512(); //todo: Bi448

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divisorLZC);
            divisorLZC += BITS_SIZE - BigInteger128.BITS_SIZE;

            var partialDivisor = divisorN.HighUInt64 + 1;

            var fullRemainder = dividend.Clone();

            while (true) {
                var remainderLZC = fullRemainder.LeadingZeroCount();
                if (remainderLZC == divisorLZC) {
                    if (fullRemainder >= divisor) {
                        fullRemainder.AssignSub(divisor);
                        quotient.AssignIncrement();
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

                // max quotient - 384 bits,
                // 64 bit <= divisor < 128 bits
                var delta = new BigInteger512();
                BigInteger128.Mul(in divisor, in guess, out delta.BiLow256);

                var guessQ = new BigInteger512(guess);
                if (correction < 0) {
                    delta.AssignLeftShift(-correction);
                    guessQ.AssignLeftShift(-correction);
                }
                fullRemainder.AssignSub(delta);
                quotient.AssignAdd(guessQ);
            }

            remainder = fullRemainder.BiLow128;
        }
   }
}