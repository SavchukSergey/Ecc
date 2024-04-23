using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public static BigInteger1024 operator *(in BigInteger512 left, in BigInteger256 right) {
            var ah = left.High;
            var al = left.Low;
            var bl = right;

            var zero = new BigInteger512(0);
            var x0 = new BigInteger1024(al * bl, zero);
            var x1 = new BigInteger1024(ah * bl, zero);
            x1.AssignLeftShiftQuarter();

            return x0 + x1;
        }

        public static BigInteger1024 operator *(in BigInteger512 left, in BigInteger512 right) {
            var ah = left.High;
            var al = left.Low;
            var bh = right.High;
            var bl = right.Low;

            var zero = new BigInteger512(0);
            var x0 = new BigInteger1024(al * bl, zero);
            var x1 = new BigInteger1024(al * bh, zero) + new BigInteger1024(ah * bl, zero);
            x1.AssignLeftShiftQuarter();
            var x2 = new BigInteger1024(zero, ah * bh);

            return x0 + x1 + x2;
        }

        /// <summary>
        /// Multiplies 512-bit and 128-bit numbers and returns last 512 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger512 MulLow(in BigInteger512 left, UInt128 right) {
            var x0 = left.Low * right;
            var x1 = BigInteger256.MulLow(left.High, right);
            x0.AssignAddHigh(x1);
            return x0;
        }


        /// <summary>
        /// Multiplies two 512-bit numbers and returns first 512 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger512 MulLow(in BigInteger512 left, in BigInteger512 right) {
            var x0 = left.Low * right.Low;
            x0.AssignAddHigh(BigInteger256.MulLow(left.Low, right.High) + BigInteger256.MulLow(left.High, right.Low));

            return x0;
        }

        /// <summary>
        /// Multiplies two 256.256 fixed point values and return 256.256 fixed point result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger512 MulFixedPoint(in BigInteger512 left, in BigInteger512 right) {
            return (left * right).Middle;
        }

    }
}