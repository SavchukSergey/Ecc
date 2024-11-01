using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger192 MulLow192(in BigInteger192 left, ulong right) {
            MulLow192(in left, right, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 MulLow256(in BigInteger192 left, in BigInteger128 right) {
            MulLow256(in left, right, out var result);
            return result;
        }

        public static void Mul(in BigInteger192 left, ulong right, out BigInteger256 result) {
            result = new BigInteger256();
            BigInteger128.Mul(left.LowUInt64, right, out result.BiLow128);
            BigInteger128.Mul(left.MiddleUInt64, right, out var x1);
            result.BiMiddle128.AssignAdd(in x1);
            BigInteger128.Mul(left.HighUInt64, right, out var x2);
            result.BiHigh128.AssignAdd(in x2);
        }

        public static void MulLow256(in BigInteger192 left, in BigInteger128 right, out BigInteger256 result) {
            Mul(in left, right.LowUInt64, out result);
            MulLow192(in left, right.HighUInt64, out var x1);
            result.BiHigh192.AssignAdd(in x1);
        }

        public static void MulLow192(in BigInteger192 left, in BigInteger192 right, out BigInteger192 result) {
            MulLow192(in left, right.LowUInt64, out result);
            BigInteger128.MulLow128(in left.BiHigh128, right.MiddleUInt64, out var x0);
            result.BiHigh128.AssignAdd(in x0);
            result.HighUInt64 += left.HighUInt64 * right.HighUInt64;
        }

        public static void MulLow192(in BigInteger192 left, ulong right, out BigInteger192 result) {
            result = new BigInteger192();
            BigInteger128.Mul(left.LowUInt64, right, out result.BiLow128);
            BigInteger128.Mul(left.MiddleUInt64, right, out var x1);
            result.BiHigh128.AssignAdd(in x1);
            result.HighUInt64 += left.HighUInt64 * right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger256 operator *(in BigInteger192 left, ulong right) {
            Mul(in left, right, out var result);
            return result;
        }
    }
}