using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger384 {

        public bool AssignAdd(in BigInteger384 other) {
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

        public static BigInteger384 operator +(in BigInteger384 left, in BigInteger384 right) {
            var res = new BigInteger384(left);
            res.AssignAdd(right);
            return res;
        }
    }
}