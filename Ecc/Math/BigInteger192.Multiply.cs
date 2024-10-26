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
            q0.AssignAddHigh(MulLow192(left, right.HighUInt64));
            return q0;
        }

        public static BigInteger192 MulLow192(in BigInteger192 left, ulong right) {
            var res = new BigInteger192(BigInteger128.Mul(left.LowUInt64, right));
            res.AssignAddHigh(BigInteger128.Mul(left.MiddleUInt64, right));
            res.AssignAddHigh(left.HighUInt64 * right);
            return res;
        }

        public static BigInteger256 operator *(in BigInteger192 left, ulong right) {
            var res = new BigInteger256(BigInteger128.Mul(left.LowUInt64, right));
            res.AssignAddMiddle(BigInteger128.Mul(left.MiddleUInt64, right));
            res.AssignAddHigh(BigInteger128.Mul(left.HighUInt64, right));
            return res;
        }
    }
}