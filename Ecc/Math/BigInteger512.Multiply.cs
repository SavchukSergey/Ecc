using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        //todo: BigInteger768
        public static BigInteger1024 operator *(in BigInteger512 left, in BigInteger256 right) {
            var ah = left.BiHigh256;
            var bl = right;

            var result = new BigInteger1024();
            BigInteger256.Mul(left.BiLow256, right, out result.BiLow512);
            var zero = new BigInteger512(0);
            var x1 = new BigInteger1024(ah * bl, zero);
            x1.AssignLeftShift256();

            return result + x1;
        }

        public static BigInteger1024 operator *(in BigInteger512 left, in BigInteger512 right) {
            var ah = left.BiHigh256;
            var al = left.BiLow256;
            var bh = right.BiHigh256;
            var bl = right.BiLow256;

            var zero = new BigInteger512(0);
            var x0 = new BigInteger1024(al * bl, zero);
            var x1 = new BigInteger1024(al * bh, zero) + new BigInteger1024(ah * bl, zero);
            x1.AssignLeftShift256();
            var x2 = new BigInteger1024(zero, ah * bh);

            return x0 + x1 + x2;
        }

        /// <summary>
        /// Multiplies 512-bit and 128-bit numbers and returns last 512 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger512 MulLow512(in BigInteger512 left, in BigInteger128 right) {
            MulLow512(in left, in right, out var result);
            return result;
        }

        public static void MulLow512(in BigInteger512 left, in BigInteger128 right, out BigInteger512 result) {
            result = new BigInteger512();
            BigInteger256.Mul(in left.BiLow256, right, out result.BiLow384);
            BigInteger256.MulLow256(in left.BiHigh256, in right, out var x1);
            result.BiHigh256.AssignAdd(x1);
        }

        /// <summary>
        /// Multiplies two 512-bit numbers and returns first 512 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static void MulLow512(in BigInteger512 left, in BigInteger512 right, out BigInteger512 result) {
            BigInteger256.Mul(in left.BiLow256, in right.BiLow256, out result);
            BigInteger256.MulLow256(left.BiLow256, right.BiHigh256, out var x1);
            result.BiHigh256.AssignAdd(in x1);
            BigInteger256.MulLow256(left.BiHigh256, right.BiLow256, out var x2);
            result.BiHigh256.AssignAdd(in x2);
        }

        public static void MulLow256(in BigInteger512 left, in BigInteger512 right, out BigInteger512 result) {
            BigInteger256.Mul(in left.BiLow256, in right.BiLow256, out result);
            BigInteger256.MulLow256(in left.BiLow256, in right.BiHigh256, out var x1);
            BigInteger256.MulLow256(in left.BiHigh256, in right.BiLow256, out var x2);
            result.BiHigh256.AssignAdd(in x1);
            result.BiHigh256.AssignAdd(in x2);
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