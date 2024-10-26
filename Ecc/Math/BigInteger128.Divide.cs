using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger128 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger128 DivRem(in BigInteger128 dividend, in BigInteger128 divisor, out BigInteger128 remainder) {
            var (q, r) = UInt128.DivRem(dividend.UInt128, divisor.UInt128);
            remainder = new BigInteger128(r);
            return new BigInteger128(q);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger128 DivRem(in BigInteger128 dividend, ulong divisor, out ulong remainder) {
            if (System.Runtime.Intrinsics.X86.X86Base.X64.IsSupported) {
                var (q1, r1) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.HighUInt64, 0, divisor);
                (var q0, remainder) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.LowUInt64, r1, divisor);
                return new BigInteger128(q0, q1);
            } else {
                var (q, r) = UInt128.DivRem(dividend.UInt128, divisor);
                remainder = (ulong)r;
                return new BigInteger128(q);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger128 DivRem(in BigInteger128 dividend, ulong divisor, out BigInteger128 remainder) {
            var (q, r) = UInt128.DivRem(dividend.UInt128, divisor);
            remainder = new BigInteger128(r);
            return new BigInteger128(q);
        }


        public static BigInteger128 operator /(in BigInteger128 left, in BigInteger128 right) {
            return DivRem(in left, in right, out var _);
        }

        public static BigInteger128 operator %(in BigInteger128 left, in BigInteger128 right) {
            DivRem(in left, in right, out var remainder);
            return remainder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger128 operator /(in BigInteger128 left, ulong right) {
            return DivRem(in left, right, out ulong _);
        }

        public static ulong operator %(in BigInteger128 left, ulong right) {
            DivRem(in left, right, out ulong remainder);
            return remainder;
        }
    }
}