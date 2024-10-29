using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger384 operator *(in BigInteger256 left, ulong right) {
            var x0 = new BigInteger384(left.BiLow128 * right);
            var x1 = new BigInteger384(0, left.BiHigh128 * right);

            return x0 + x1;
        }

        public static BigInteger512 operator *(in BigInteger256 left, in BigInteger256 right) {
            var res = new BigInteger512(left.BiLow128 * right.BiLow128);
            var x1 = new BigInteger512(left.BiLow128 * right.BiHigh128) + new BigInteger512(left.BiHigh128 * right.BiLow128);
            x1.AssignLeftShiftQuarter();
            res.AssignAddHigh(left.BiHigh128 * right.BiHigh128);

            return res + x1;
        }

        public static BigInteger384 operator *(in BigInteger256 left, in BigInteger128 right) {
            var x0 = new BigInteger384(left.BiLow128 * right);
            var x1 = new BigInteger384(0ul, left.BiHigh128 * right);

            return x0 + x1;
        }

        public static void Mul(in BigInteger256 left, in BigInteger256 right, out BigInteger512 result) {
            result = new BigInteger512(left.BiLow128 * right.BiLow128);
            var x1 = new BigInteger512(left.BiLow128 * right.BiHigh128) + new BigInteger512(left.BiHigh128 * right.BiLow128);
            x1.AssignLeftShiftQuarter();
            result.AssignAdd(x1);
            result.High.AssignAdd(left.BiHigh128 * right.BiHigh128);
        }

        /// <summary>
        /// Multiplies 256-bit and 64-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulLow256(in BigInteger256 left, ulong right) {
            var x0 = new BigInteger256(left.BiLow128 * right);
            var x1 = BigInteger128.MulLow128(left.BiHigh128, right);
            x0.AssignAddHigh(x1);
            return x0;
        }

        /// <summary>
        /// Multiplies 256-bit and 64-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulLow256(in BigInteger256 left, UInt128 right) {
            var x0 = left.BiLow128 * right;
            var x1 = BigInteger128.MulLow128(left.BiHigh128, right);
            x0.AssignAddHigh(x1);
            return x0;
        }

        /// <summary>
        /// Multiplies 256-bit and 128-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulLow256(in BigInteger256 left, in BigInteger128 right) {
            var x0 = left.BiLow128 * right;
            var x1 = BigInteger128.MulLow128(left.BiHigh128, right);
            x0.AssignAddHigh(x1);
            return x0;
        }

        /// <summary>
        /// Multiplies two 256-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 MulLow256(in BigInteger256 left, in BigInteger256 right) {
            MulLow256(in left, in right, out var result);
            return result;
        }

        public static void MulLow256(in BigInteger256 left, in BigInteger256 right, out BigInteger256 result) {
            result = left.BiLow128 * right.BiLow128;
            var x1 = BigInteger128.MulLow128(in left.BiLow128, in right.BiHigh128);
            var x2 = BigInteger128.MulLow128(in left.BiHigh128, in right.BiLow128);

            result.BiHigh128.AssignAdd(x1);
            result.BiHigh128.AssignAdd(x2);
        }


        /// <summary>
        /// Multiplies two 256-bit numbers and returns highest 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger256 MulHigh256(in BigInteger256 left, in BigInteger256 right) {
            return (left * right).High;
        }

        public readonly BigInteger512 Square() {
            var res = new BigInteger512(BiLow128.Square());

            var mid = new BigInteger512(BiLow128 * BiHigh128);
            mid.AssignLeftShiftQuarter();
            mid.AssignDouble();

            res.AssignAddHigh(BiHigh128.Square());

            return res + mid;
        }

    }
}