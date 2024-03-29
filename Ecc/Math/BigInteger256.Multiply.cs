using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger512 operator *(in BigInteger256 left, ulong right) {
            var ah = left.High;
            var al = left.Low;
            var bl = right;

            var x0 = new BigInteger512(Mul128(al, bl));
            var x1 = new BigInteger512(Mul128(ah, bl));
            x1.AssignLeftShiftQuarter();

            return x0 + x1;
        }


        public static BigInteger512 operator *(in BigInteger256 left, in UInt128 right) {
            if (left.IsZero || right == 0) {
                return new BigInteger512(0);
            }
            if (left.IsOne) {
                return new BigInteger512(right);
            }
            if (right == 1) {
                return new BigInteger512(left);
            }

            var ah = left.High;
            var al = left.Low;
            var bl = right;

            var x0 = new BigInteger512(Mul128(al, bl));
            var x1 = new BigInteger512(Mul128(ah, bl));
            x1.AssignLeftShiftQuarter();

            return x0 + x1;
        }

        public static BigInteger512 operator *(in BigInteger256 left, in BigInteger256 right) {
            var ah = left.High;
            var al = left.Low;
            var bh = right.High;
            var bl = right.Low;

            var zero = new BigInteger256(0);
            var x0 = new BigInteger512(Mul128(al, bl));
            var x1 = new BigInteger512(Mul128(al, bh)) + new BigInteger512(Mul128(ah, bl));
            x1.AssignLeftShiftQuarter();
            var x2 = new BigInteger512(zero, Mul128(ah, bh));

            return x0 + x1 + x2;
        }

        /// <summary>
        /// Multiplies 256-bit and 64-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulLow(in BigInteger256 left, ulong right) {
            var x0 = Mul128(left.Low, right);
            x0.AssignAddHigh(Mul128Low(left.High, right));
            return x0;
        }

        /// <summary>
        /// Multiplies 256-bit and 128-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulLow(in BigInteger256 left, UInt128 right) {
            var x0 = Mul128(left.Low, right);
            x0.AssignAddHigh(Mul128Low(left.High, right));
            return x0;
        }

        /// <summary>
        /// Multiplies two 256-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulLow(in BigInteger256 left, in BigInteger256 right) {
            var x0 = Mul128(left.Low, right.Low);
            x0.AssignAddHigh(Mul128Low(left.Low, right.High) + Mul128Low(left.High, right.Low));

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
            return this * this;
        }

    }
}