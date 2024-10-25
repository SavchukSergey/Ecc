using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger384 operator *(in BigInteger256 left, ulong right) {
            var ah = left.BiHigh;
            var al = left.BiLow;
            var bl = right;

            var x0 = new BigInteger384(al * bl);
            var x1 = new BigInteger384(0, ah * bl);

            return x0 + x1;
        }

        public static BigInteger512 operator *(in BigInteger256 left, in BigInteger256 right) {
            var x0 = new BigInteger512(left.BiLow * right.BiLow);
            var x1 = new BigInteger512(left.BiLow * right.BiHigh) + new BigInteger512(left.BiHigh * right.BiLow);
            x1.AssignLeftShiftQuarter();
            var x2 = new BigInteger512(Zero, left.BiHigh * right.BiHigh);

            return x0 + x1 + x2;
        }

        public static BigInteger384 operator *(in BigInteger256 left, in BigInteger128 right) {
            var x0 = new BigInteger384(left.BiLow * right);
            var x1 = new BigInteger384(0, left.BiHigh * right);

            return x0 + x1;
        }

        /// <summary>
        /// Multiplies 256-bit and 64-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulLow(in BigInteger256 left, ulong right) {
            var x0 = new BigInteger256(left.BiLow128 * right);
            var x1 = BigInteger128.MulLow(left.BiHigh128, right);
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
            var low = new BigInteger512(BiLow.Square());

            var mid = new BigInteger512(BiLow * BiHigh);
            mid.AssignLeftShiftQuarter();
            mid.AssignDouble();

            var high = new BigInteger512(Zero, BiHigh.Square());

            return low + mid + high;
        }

    }
}