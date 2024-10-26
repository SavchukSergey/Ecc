using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger128 {

        public static BigInteger192 Mul(in BigInteger128 left, ulong right) {
            var ah = left.HighUInt64;
            var al = left.LowUInt64;
            var bl = right;

            var res = new BigInteger192(Mul(al, bl));
            res.AssignAddHigh(Mul(ah, bl));
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger192 operator *(in BigInteger128 left, ulong right) {
            return Mul(left, right);
        }

        public static BigInteger256 Mul(in BigInteger128 left, in BigInteger128 right) {
            if (left.IsZero || right.IsZero) {
                return new BigInteger256(0);
            }
            if (left.IsOne) {
                return new BigInteger256(right);
            }
            if (right.IsOne) {
                return new BigInteger256(left);
            }
            var ah = left.High;
            var al = left.Low;
            var bh = right.High;
            var bl = right.Low;

            var x0 = new BigInteger256(Mul(al, bl));
            var x1 = new BigInteger256(Mul(al, bh)) + new BigInteger256(Mul(ah, bl));
            x1.AssignLeftShiftQuarter();
            var x2 = new BigInteger256(new BigInteger128(), Mul(ah, bh));

            return x0 + x1 + x2;
        }

        public static BigInteger192 MulLow192(in BigInteger128 left, in BigInteger128 right) {
            var ah = left.High;
            var al = left.Low;
            var bh = right.High;
            var bl = right.Low;

            var res = new BigInteger192(Mul(al, bl));
            res.AssignAddHigh(Mul(al, bh) + Mul(ah, bl));//do not care about overflow
            return res;
        }

        public static BigInteger128 MulLow128(in BigInteger128 left, in BigInteger128 right) {
            var ah = left.HighUInt64;
            var al = left.LowUInt64;
            var bh = right.HighUInt64;
            var bl = right.LowUInt64;

            var res = Mul(al, bl);
            res.AssignAddHigh(al * bh);
            res.AssignAddHigh(ah * bl);

            return res;
        }

        public static BigInteger128 MulLow128(in BigInteger128 left, UInt128 right) {
            var ah = left.HighUInt64;
            var al = left.LowUInt64;
            var bh = (ulong)(right >> 64);
            var bl = (ulong)right;

            var x0 = Mul(al, bl);
            var x1 = MulLow(al, bh);
            var x2 = MulLow(ah, bl);
            x0.AssignAddHigh(x1);
            x0.AssignAddHigh(x2);

            return x0;
        }

        public static BigInteger128 MulLow128(in BigInteger128 left, ulong right) {
            var res = Mul(left.LowUInt64, right);
            res.AssignAddHigh(left.HighUInt64 * right);
            return res;
        }

        public static BigInteger128 Mul(ulong left, ulong right) {
            if (System.Runtime.Intrinsics.X86.Bmi2.IsSupported) {
                ulong low = 0;
                var high = System.Runtime.Intrinsics.X86.Bmi2.X64.MultiplyNoFlags(left, right, &low);
                return new BigInteger128(low, high);
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

                return new BigInteger128(x0 + x1 + x2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong MulLow(ulong left, ulong right) {
            return left * right;
        }

        public static BigInteger256 operator *(in BigInteger128 left, in UInt128 right) {
            if (left.IsZero || right == 0) {
                return new BigInteger256(0);
            }
            if (left.IsOne) {
                return new BigInteger256(right);
            }
            if (right == 1) {
                return new BigInteger256(left);
            }
            var ah = left.High;
            var al = left.Low;
            var bh = (ulong)(right >> 64);
            var bl = (ulong)right;

            var x0 = new BigInteger256(Mul(al, bl));
            var x1 = new BigInteger256(Mul(al, bh)) + new BigInteger256(Mul(ah, bl));
            x1.AssignLeftShiftQuarter();
            var x2 = new BigInteger256(new BigInteger128(), Mul(ah, bh));

            return x0 + x1 + x2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 operator *(in BigInteger128 left, in BigInteger128 right) {
            return Mul(left, right);
        }

        public readonly BigInteger256 Square() {
            if (UInt128 == 0 || UInt128 == 1) {
                return new BigInteger256(UInt128);
            }
            var ah = UInt128 >> 64;
            var al = (UInt128)(ulong)UInt128;

            var low = new BigInteger256(al * al);

            var mid = new BigInteger256(al * ah);
            mid.AssignLeftShiftQuarter();
            mid.AssignDouble();

            var high = new BigInteger256(0, ah * ah);

            return low + mid + high;
        }
    }
}