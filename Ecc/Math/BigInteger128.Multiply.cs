using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger128 {

        public static BigInteger256 Mul128(UInt128 left, ulong right) {
            var ah = left >> 64;
            var al = (UInt128)(ulong)left;
            var bl = (UInt128)right;

            var x0 = new BigInteger256(al * bl, 0);
            var x1 = new BigInteger256(ah * bl, 0);
            x1.AssignLeftShiftQuarter();

            return x0 + x1;
        }

        public static BigInteger256 Mul128(UInt128 left, UInt128 right) {
            if (left == 0 || right == 0) {
                return new BigInteger256(0);
            }
            if (left == 1) {
                return new BigInteger256(right);
            }
            if (right == 1) {
                return new BigInteger256(left);
            }
            var ah = left >> 64;
            var al = (UInt128)(ulong)left;
            var bh = right >> 64;
            var bl = (UInt128)(ulong)right;

            var x0 = new BigInteger256(al * bl, 0);
            var x1 = new BigInteger256(al * bh, 0) + new BigInteger256(ah * bl, 0);
            x1.AssignLeftShiftQuarter();
            var x2 = new BigInteger256(0, ah * bh);

            return x0 + x1 + x2;
        }

        public static UInt128 Mul128Low(UInt128 left, UInt128 right) {
            var ah = left >> 64;
            var al = (UInt128)(ulong)left;
            var bh = right >> 64;
            var bl = (UInt128)(ulong)right;

            var x0 = al * bl;
            var x1 = (al * bh + ah * bl) << 64; //todo: we can use Mul64Low here check if it is faster

            return x0 + x1;
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

            var high = new BigInteger256(0, ah * ah);

            return low + mid + mid + high;
        }
    }
}