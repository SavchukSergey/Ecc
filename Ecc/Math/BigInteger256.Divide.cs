using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static void DivRem(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 quotient, out BigInteger256 remainder) {
            if (divisor.HighUInt64 == 0) {
                remainder = new BigInteger256();
                DivRem(in dividend, in divisor.BiLow192, out quotient, out remainder.BiLow192);
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
            MulLow256(divisor, q64, out var delta);
            remainder.AssignSub(delta);

            if (remainder >= divisor) {
                remainder.AssignSub(divisor);
                q64++;

                if (remainder >= divisor) {
                    remainder.AssignSub(divisor);
                    q64++;
                }
            }
            quotient = new BigInteger256(q64);
        }

        public static void DivRem(in BigInteger256 dividend, ulong divisor, out BigInteger256 quotient, out ulong remainder) {
            if (divisor <= uint.MaxValue) {
                DivRem(dividend, (uint)divisor, out quotient, out uint remainder32);
                remainder = remainder32;
                return;
            }
            if (dividend.BiHigh128.IsZero) {
                quotient = new BigInteger256();
                BigInteger128.DivRem(in dividend.BiLow128, divisor, out quotient.BiLow128, out remainder);
                return;
            }

            if (System.Runtime.Intrinsics.X86.X86Base.X64.IsSupported) {
#pragma warning disable SYSLIB5004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                var (q3, r3) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[3], 0, divisor);
                var (q2, r2) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[2], r3, divisor);
                var (q1, r1) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[1], r2, divisor);
                (var q0, remainder) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[0], r1, divisor);
#pragma warning restore SYSLIB5004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                quotient = new BigInteger256(q0, q1, q2, q3);
            } else {
                quotient = new BigInteger256();
                var rem = new UInt128();
                for (var i = UINT64_SIZE - 1; i >= 0; i--) {
                    var partialDividend = (rem << 64) + dividend.UInt64[i];
                    var (partialQuotient, partialRemainder) = UInt128.DivRem(partialDividend, divisor);
                    quotient.UInt64[i] = (ulong)partialQuotient;
                    rem = partialRemainder;
                }
                remainder = (ulong)rem;
            }
        }

        public static void DivRem(in BigInteger256 dividend, uint divisor, out BigInteger256 quotient, out uint remainder) {
            quotient = new BigInteger256();
            if (dividend.BiHigh128.IsZero) {
                BigInteger128.DivRem(in dividend.BiLow128, divisor, out quotient.BiLow128, out remainder);
                return;
            }
            var rem = 0ul;
            for (var i = UINT32_SIZE - 1; i >= 0; i--) {
                var partialDividend = (rem << 32) + dividend.UInt32[i];
                var (partialQuotient, partialRemainder) = ulong.DivRem(partialDividend, divisor);
                quotient.UInt32[i] = (uint)partialQuotient;
                rem = partialRemainder;
            }
            remainder = (uint)rem;
        }

        public static BigInteger256 operator /(in BigInteger256 left, in BigInteger256 right) {
            DivRem(left, right, out var quotient, out var _);
            return quotient;
        }

        public static BigInteger256 operator %(in BigInteger256 left, in BigInteger256 right) {
            DivRem(left, right, out var _, out var remainder);
            return remainder;
        }

    }
}