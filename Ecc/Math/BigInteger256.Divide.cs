using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRem(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            return DivRemGuess(in dividend, in divisor, out remainder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRem(in BigInteger256 dividend, in BigInteger192 divisor, out BigInteger192 remainder) {
            return DivRemGuess(in dividend, in divisor, out remainder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRem(in BigInteger256 dividend, in BigInteger128 divisor, out BigInteger128 remainder) {
            return DivRemGuess(in dividend, in divisor, out remainder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRem(in BigInteger256 dividend, in BigInteger192 divisor, out BigInteger256 remainder) {
            var res = DivRem(in dividend, in divisor, out BigInteger192 remainder192);
            remainder = new BigInteger256(remainder192);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRem(in BigInteger256 dividend, in BigInteger128 divisor, out BigInteger256 remainder) {
            var res = DivRem(in dividend, in divisor, out BigInteger128 remainder128);
            remainder = new BigInteger256(remainder128);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRem(in BigInteger256 dividend, in BigInteger128 divisor, out BigInteger192 remainder) {
            var res = DivRem(in dividend, divisor, out BigInteger128 remainder128);
            remainder = new BigInteger192(remainder128);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRem(in BigInteger256 dividend, ulong divisor, out BigInteger256 remainder) {
            var res = DivRem(in dividend, divisor, out ulong remainder64);
            remainder = new BigInteger256(remainder64);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRem(in BigInteger256 dividend, ulong divisor, out BigInteger192 remainder) {
            var res = DivRem(in dividend, divisor, out ulong remainder64);
            remainder = new BigInteger192(remainder64);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRem(in BigInteger256 dividend, ulong divisor, out BigInteger128 remainder) {
            var res = DivRem(in dividend, divisor, out ulong remainder64);
            remainder = new BigInteger128(remainder64);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRem(in BigInteger256 dividend, uint divisor, out BigInteger256 remainder) {
            var res = DivRem(in dividend, divisor, out uint remainder32);
            remainder = new BigInteger256(remainder32);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRem(in BigInteger256 dividend, uint divisor, out BigInteger192 remainder) {
            var res = DivRem(in dividend, divisor, out uint remainder32);
            remainder = new BigInteger192(remainder32);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 DivRem(in BigInteger256 dividend, uint divisor, out BigInteger128 remainder) {
            var res = DivRem(in dividend, divisor, out uint remainder32);
            remainder = new BigInteger128(remainder32);
            return res;
        }

        public static BigInteger256 DivRem(in BigInteger256 dividend, ulong divisor, out ulong remainder) {
            if (divisor <= uint.MaxValue) {
                var res = DivRem(dividend, (uint)divisor, out uint remainder32);
                remainder = remainder32;
                return res;
            }

            if (System.Runtime.Intrinsics.X86.X86Base.X64.IsSupported) {
                var (q3, r3) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[3], 0, divisor);
                var (q2, r2) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[2], r3, divisor);
                var (q1, r1) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[1], r2, divisor);
                (var q0, remainder) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[0], r1, divisor);
                return new BigInteger256(q0, q1, q2, q3);
            } else {
                var quotient = new BigInteger256();
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

        public static BigInteger256 DivRem(in BigInteger256 dividend, uint divisor, out uint remainder) {
            var quotient = new BigInteger256();
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

        public static BigInteger256 operator /(in BigInteger256 left, in BigInteger256 right) {
            return DivRem(left, right, out var _);
        }

        public static BigInteger256 operator %(in BigInteger256 left, in BigInteger256 right) {
            DivRem(left, right, out var remainder);
            return remainder;
        }
    }
}