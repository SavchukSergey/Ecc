using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        public static BigInteger256 Mul(in BigInteger192 left, ulong right) {
           var res = new BigInteger256(BigInteger128.Mul(left.LowUInt64, right));
            res.AssignAddMiddle(BigInteger128.Mul(left.MiddleUInt64, right));
            res.AssignAddHigh(BigInteger128.Mul(left.HighUInt64, right));
            return res;
        }

        public static BigInteger256 MulLow256(in BigInteger192 left, in BigInteger128 right) {
            var q0 = left * right.LowUInt64;
            MulLow192(in left, right.HighUInt64, out var x1);
            q0.BiHigh192.AssignAdd(in x1);
            return q0;
        }

        public static void MulLow256(in BigInteger192 left, in BigInteger128 right, out BigInteger256 result) {
            result = left * right.LowUInt64;
            MulLow192(in left, right.HighUInt64, out var x1);
            result.BiHigh192.AssignAdd(in x1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger192 MulLow192(in BigInteger192 left, ulong right) {
            MulLow192(in left, right, out var result);
            return result;
        }

        public static void MulLow192(in BigInteger192 left, ulong right, out BigInteger192 result) {
            result = new BigInteger192();
            BigInteger128.Mul(left.LowUInt64, right, out result.BiLow128);
            BigInteger128.Mul(left.MiddleUInt64, right, out var x1);
            result.BiHigh128.AssignAdd(in x1);
            result.AssignAddHigh(left.HighUInt64 * right);
        }

        public static BigInteger256 operator *(in BigInteger192 left, ulong right) {
            var res = new BigInteger256(BigInteger128.Mul(left.LowUInt64, right));
            res.AssignAddMiddle(BigInteger128.Mul(left.MiddleUInt64, right));
            res.AssignAddHigh(BigInteger128.Mul(left.HighUInt64, right));
            return res;
        }
    }
}