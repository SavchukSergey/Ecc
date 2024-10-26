using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public static BigInteger256 operator %(in BigInteger512 left, in BigInteger256 right) {
            DivRem(left, right, out BigInteger256 remainder);
            return remainder;
        }

        public static BigInteger512 DivRem(in BigInteger512 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            return DivRemGuess(dividend, divisor, out remainder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger512 DivRem(in BigInteger512 dividend, in BigInteger512 divisor, out BigInteger512 remainder) {
            return DivRemGuess(dividend, divisor, out remainder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger512 DivRem(in BigInteger512 dividend, ulong divisor, out BigInteger512 remainder) {
            var res = DivRem(in dividend, divisor, out ulong remainder64);
            remainder = new BigInteger512(remainder64);
            return res;
        }

        public static BigInteger512 DivRem(in BigInteger512 dividend, ulong divisor, out ulong remainder) {
            if (divisor <= uint.MaxValue) {
                var res = DivRem(dividend, (uint)divisor, out uint remainder32);
                remainder = remainder32;
                return res;
            }
            if (System.Runtime.Intrinsics.X86.X86Base.X64.IsSupported) {
                var (q7, r7) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[7], 0, divisor);
                var (q6, r6) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[6], r7, divisor);
                var (q5, r5) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[5], r6, divisor);
                var (q4, r4) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[4], r5, divisor);
                var (q3, r3) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[3], r4, divisor);
                var (q2, r2) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[2], r3, divisor);
                var (q1, r1) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[1], r2, divisor);
                (var q0, remainder) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[0], r1, divisor);
                return new BigInteger512(q0, q1, q2, q3, q4, q5, q6, q7);
            } else {
                var quotient = new BigInteger512();
                var rem = new UInt128();
                for (var i = UINT64_SIZE - 1; i >= 0; i--) {
                    var partialDividend = (rem << 64) + dividend.UInt64[i];
                    var (partialQuotient, partialRemainder) = UInt128.DivRem(partialDividend, divisor);
                    quotient.UInt64[i] = (ulong)partialQuotient;
                    rem = partialRemainder;
                }

                remainder = (ulong)rem;

                return quotient;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger512 DivRem(in BigInteger512 dividend, uint divisor, out BigInteger512 remainder) {
            var res = DivRem(in dividend, divisor, out uint remainder32);
            remainder = new BigInteger512(remainder32);
            return res;
        }

        public static BigInteger512 DivRem(in BigInteger512 dividend, uint divisor, out uint remainder) {
            var quotient = new BigInteger512();
            var rem = 0ul;
            for (var i = UINT32_SIZE - 1; i >= 0; i--) {
                var partialDividend = (rem << 32) + dividend.UInt32[i];
                var (partialQuotient, partialRemainder) = ulong.DivRem(partialDividend, divisor);
                quotient.UInt32[i] = (uint)partialQuotient;
                rem = partialRemainder;
            }
            remainder = (uint)rem;

            return quotient;
        }
    }
}