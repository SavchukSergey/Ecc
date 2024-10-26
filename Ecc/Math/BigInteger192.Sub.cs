namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        public static BigInteger192 operator -(in BigInteger192 left, in BigInteger192 right) {
            var res = left;
            var borrow = left.LowUInt128 < right.LowUInt128;
            res.LowUInt128 -= right.LowUInt128;
            res.HighUInt64 -= right.HighUInt64;
            if (borrow) {
                res.HighUInt64--;
            }
            return res;
        }

        public void AssignSub(in BigInteger192 other) {
            var borrow = LowUInt128 < other.LowUInt128;
            LowUInt128 -= other.LowUInt128;
            HighUInt64 -= other.HighUInt64;
            if (borrow) {
                HighUInt64--;
            }
        }

        public void AssignSub(in BigInteger128 other) {
            var borrow = LowUInt128 < other.UInt128;
            LowUInt128 -= other.UInt128;
            if (borrow) {
                HighUInt64--;
            }
        }

    }
}