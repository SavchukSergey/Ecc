using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        public bool AssignAdd(in BigInteger192 other) {
            byte carry = 0;
            for (var i = 0; i < UINT64_SIZE; i++) {
                UInt128 acc = UInt64[i];
                acc += other.UInt64[i];
                acc += carry;
                UInt64[i] = (ulong)acc;
                carry = (byte)(acc >> 64);
            }
            return carry > 0;
        }

        public void AssignAddHigh(in BigInteger128 other) {
            HighUInt128 += other.UInt128;
        }

        public void AssignAddHigh(ulong val) {
            HighUInt64 += val;
        }

        public static BigInteger192 operator +(in BigInteger192 left, in BigInteger192 right) {
            var res = new BigInteger192(left);
            res.AssignAdd(right);
            return res;
        }
    }
}