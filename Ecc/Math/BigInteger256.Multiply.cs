using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        /// <summary>
        /// Multiplies 256-bit and 64-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 MulLow256(in BigInteger256 left, ulong right) {
            MulLow256(in left, right, out var result);
            return result;
        }

        /// <summary>
        /// Multiplies 256-bit and 128-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 MulLow256(in BigInteger256 left, in BigInteger128 right) {
            MulLow256(in left, in right, out var result);
            return result;
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

        public static BigInteger384 operator *(in BigInteger256 left, ulong right) {
            var x0 = new BigInteger384(left.BiLow128 * right);
            var x1 = new BigInteger384(0, left.BiHigh128 * right);

            return x0 + x1;
        }

        public static BigInteger512 operator *(in BigInteger256 left, in BigInteger256 right) {
            Mul(in left, in right, out var result);
            return result;
        }

        public static BigInteger384 operator *(in BigInteger256 left, in BigInteger128 right) {
            var x0 = new BigInteger384(left.BiLow128 * right);
            var x1 = new BigInteger384(0ul, left.BiHigh128 * right);

            return x0 + x1;
        }

        public static void Mul(in BigInteger256 left, in BigInteger256 right, out BigInteger512 result) {
            result = new BigInteger512();
            BigInteger128.Mul(in left.BiLow128, in right.BiLow128, out result.BiLow256);
            BigInteger128.Mul(in left.BiLow128, in right.BiHigh128, out var x1);
            BigInteger128.Mul(in left.BiHigh128, in right.BiLow128, out var x2);
            result.BiHigh384.AssignAdd(in x1);
            result.BiHigh384.AssignAdd(in x2);
            BigInteger128.Mul(in left.BiHigh128, in right.BiHigh128, out var x3);
            result.BiHigh256.AssignAdd(in x3);
        }


        /// <summary>
        /// Multiplies 256-bit and 64-bit numbers and returns last 256 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static void MulLow256(in BigInteger256 left, ulong right, out BigInteger256 result) {
            result = new BigInteger256();
            BigInteger128.Mul(in left.BiLow128, right, out result.BiLow192);
            BigInteger128.MulLow128(in left.BiHigh128, right, out var x1);
            result.BiHigh128.AssignAdd(in x1);
        }

        public static void MulLow256(in BigInteger256 left, in BigInteger128 right, out BigInteger256 result) {
            BigInteger128.Mul(in left.BiLow128, in right, out result);
            BigInteger128.MulLow128(in left.BiHigh128, right, out var x1);
            result.BiHigh128.AssignAdd(in x1);
        }

        public static void MulLow256(in BigInteger256 left, in BigInteger256 right, out BigInteger256 result) {
            BigInteger128.Mul(in left.BiLow128, in right.BiLow128, out result);
            BigInteger128.MulLow128(in left.BiLow128, in right.BiHigh128, out var x1);
            BigInteger128.MulLow128(in left.BiHigh128, in right.BiLow128, out var x2);
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
            return (left * right).BiHigh256;
        }

        public static void Mul(in BigInteger256 left, in BigInteger128 right, out BigInteger384 result) {
            result = new BigInteger384();
            BigInteger128.Mul(in left.BiLow128, right, out result.BiLow256);
            BigInteger128.Mul(in left.BiHigh128, right, out var x1);
            result.BiHigh256.AssignAdd(in x1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly BigInteger512 Square() {
            Square(out var result);
            return result;
        }

        public readonly void Square(out BigInteger512 result) {
            result = new BigInteger512();
            BiLow128.Square(out result.BiLow256);

            BigInteger128.Mul(in BiLow128, in BiHigh128, out var x1);
            result.BiHigh384.AssignAdd(in x1);
            result.BiHigh384.AssignAdd(in x1);

            BiHigh128.Square(out var x2);
            result.BiHigh256.AssignAdd(in x2);
        }

    }
}