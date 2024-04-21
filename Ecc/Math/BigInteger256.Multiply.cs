using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger512 operator *(in BigInteger256 left, ulong right) {
            var ah = left.BiHigh;
            var al = left.BiLow;
            var bl = right;

            var x0 = new BigInteger512(al * bl);
            var x1 = new BigInteger512(ah * bl);
            x1.AssignLeftShiftQuarter();

            return x0 + x1;
        }

        public static BigInteger512 operator *(in BigInteger256 left, in BigInteger256 right) {
            var ah = left.BiHigh;
            var al = left.BiLow;
            var bh = right.BiHigh;
            var bl = right.BiLow;

            var zero = new BigInteger256(0);
            var x0 = new BigInteger512(al * bl);
            var x1 = new BigInteger512(al * bh) + new BigInteger512(ah * bl);
            x1.AssignLeftShiftQuarter();
            var x2 = new BigInteger512(zero, ah * bh);

            return x0 + x1 + x2;
        }

        public static BigInteger512 operator *(in BigInteger256 left, in BigInteger128 right) {
            var ah = left.BiHigh;
            var al = left.BiLow;
            var bl = right;

            var x0 = new BigInteger512(al * bl);
            var x1 = new BigInteger512(ah * bl);
            x1.AssignLeftShiftQuarter();

            return x0 + x1;
        }

        public static BigInteger512 operator *(in BigInteger256 left, in UInt128 right) {
            var ah = left.BiHigh;
            var al = left.BiLow;
            var bl = right;

            var x0 = new BigInteger512(al * bl);
            var x1 = new BigInteger512(ah * bl);
            x1.AssignLeftShiftQuarter();

            return x0 + x1;
        }

        /// <summary>
        /// Multiplies 256-bit and 64-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulLow(in BigInteger256 left, ulong right) {
            var x0 = left.BiLow * right;
            var x1 = BigInteger128.MulLow(left.BiHigh, right);
            x0.AssignAddHigh(x1);
            return x0;
        }

        /// <summary>
        /// Multiplies 256-bit and 64-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulLow(in BigInteger256 left, UInt128 right) {
            var x0 = left.BiLow * right;
            var x1 = BigInteger128.MulLow(left.BiHigh, right);
            x0.AssignAddHigh(x1);
            return x0;
        }

        /// <summary>
        /// Multiplies 256-bit and 128-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulLow(in BigInteger256 left, BigInteger128 right) {
            var x0 = left.BiLow * right;
            var x1 = BigInteger128.MulLow(left.BiHigh, right);
            x0.AssignAddHigh(x1);
            return x0;
        }

        /// <summary>
        /// Multiplies two 256-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulLow(in BigInteger256 left, in BigInteger256 right) {
            var x0 = left.BiLow * right.BiLow;
            var x1 = BigInteger128.MulLow(left.BiLow, right.BiHigh);
            var x2 = BigInteger128.MulLow(left.BiHigh, right.BiLow);

            x0.AssignAddHigh(x1);
            x0.AssignAddHigh(x2);

            return x0;
        }

        /// <summary>
        /// Multiplies two 256-bit numbers and returns first 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulHigh(in BigInteger256 left, in BigInteger256 right) {
            return (left * right).High;
        }

        public readonly BigInteger512 Square() {
            var zero = new BigInteger256(0);

            var low = new BigInteger512(BiLow.Square());

            var mid = new BigInteger512(BiLow * BiHigh);
            mid.AssignLeftShiftQuarter();

            var high = new BigInteger512(zero, BiHigh.Square());

            return low + mid + mid + high;
        }

    }
}