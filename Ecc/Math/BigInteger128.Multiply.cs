using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger128 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger192 Mul(in BigInteger128 left, ulong right) {
            Mul(left, right, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 Mul(in BigInteger128 left, in BigInteger128 right) {
            Mul(in left, in right, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger128 MulLow128(in BigInteger128 left, in BigInteger128 right) {
            MulLow128(in left, in right, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger128 MulLow128(in BigInteger128 left, ulong right) {
            MulLow128(in left, right, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger128 Mul(ulong left, ulong right) {
            Mul(left, right, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly BigInteger256 Square() {
            Square(out var result);
            return result;
        }

        public static void Mul(in BigInteger128 left, ulong right, out BigInteger192 result) {
            result = new BigInteger192();
            Mul(left.LowUInt64, right, out result.BiLow128);
            Mul(left.HighUInt64, right, out var x1);
            result.BiHigh128.AssignAdd(in x1);
        }

        public static void Mul(in BigInteger128 left, in BigInteger128 right, out BigInteger256 result) {
            if (left.IsZero || right.IsZero) {
                result = new BigInteger256(0);
                return;
            }
            if (left.IsOne) {
                result = new BigInteger256(in right);
                return;
            }
            if (right.IsOne) {
                result = new BigInteger256(in left);
                return;
            }

            result = new BigInteger256();
            Mul(left.LowUInt64, right.LowUInt64, out result.BiLow128);
            Mul(left.LowUInt64, right.HighUInt64, out var x1);
            result.BiHigh192.AssignAdd(in x1);
            Mul(left.HighUInt64, right.LowUInt64, out var x2);
            result.BiHigh192.AssignAdd(in x2);
            Mul(left.HighUInt64, right.HighUInt64, out var x3);
            result.BiHigh128.AssignAdd(in x3);
        }

        public static void MulLow192(in BigInteger128 left, in BigInteger128 right, out BigInteger192 result) {
            result = new BigInteger192();
            Mul(left.LowUInt64, right.LowUInt64, out result.BiLow128);
            Mul(left.LowUInt64, right.HighUInt64, out var x1);
            Mul(left.HighUInt64, right.LowUInt64, out var x2);
            result.BiHigh128.AssignAdd(in x1);
            result.BiHigh128.AssignAdd(in x2);
        }

        public static void MulLow128(in BigInteger128 left, in BigInteger128 right, out BigInteger128 result) {
            Mul(left.LowUInt64, right.LowUInt64, out result);
            result.HighUInt64 += left.LowUInt64 * right.HighUInt64;
            result.HighUInt64 += left.HighUInt64 * right.LowUInt64;
        }

        public static void MulLow128(in BigInteger128 left, ulong right, out BigInteger128 result) {
            Mul(left.LowUInt64, right, out result);
            result.HighUInt64 += left.HighUInt64 * right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Mul(ulong left, ulong right, out BigInteger128 result) {
            if (System.Runtime.Intrinsics.X86.Bmi2.IsSupported) {
                ulong low = 0;
                var high = System.Runtime.Intrinsics.X86.Bmi2.X64.MultiplyNoFlags(left, right, &low);
                result = new BigInteger128(low, high);
            } else {
                var ah = left >> 32;
                var al = (ulong)(uint)left;
                var bh = right >> 32;
                var bl = (ulong)(uint)right;

                var x0 = (UInt128)(al * bl);
                var x1 = (UInt128)(al * bh) + (UInt128)(ah * bl);
                x1 <<= 32;
                var x2 = (UInt128)(ah * bh);
                x2 <<= 64;

                result = new BigInteger128(x0 + x1 + x2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger192 operator *(in BigInteger128 left, ulong right) {
            return Mul(left, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 operator *(in BigInteger128 left, in BigInteger128 right) {
            Mul(left, right, out var result);
            return result;
        }

        public readonly void Square(out BigInteger256 result) {
            if (UInt128 == 0 || UInt128 == 1) {
                result = new BigInteger256(UInt128);
                return;
            }
            result = new BigInteger256();
            Mul(LowUInt64, LowUInt64, out result.BiLow128);
            Mul(LowUInt64, HighUInt64, out var x1);
            result.BiHigh192.AssignAdd(in x1);
            result.BiHigh192.AssignAdd(in x1);

            Mul(HighUInt64, HighUInt64, out var x2);
            result.BiHigh128.AssignAdd(in x2);
        }
    }
}