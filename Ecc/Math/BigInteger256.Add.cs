using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public bool AssignAdd(in BigInteger256 other) {
            UInt128 acc = UInt64[0];
            acc += other.UInt64[0];
            UInt64[0] = (ulong)acc;
            acc >>= 64;

            acc += UInt64[1];
            acc += other.UInt64[1];
            UInt64[1] = (ulong)acc;
            acc >>= 64;

            acc += UInt64[2];
            acc += other.UInt64[2];
            UInt64[2] = (ulong)acc;
            acc >>= 64;

            acc += UInt64[3];
            acc += other.UInt64[3];
            UInt64[3] = (ulong)acc;
            acc >>= 64;

            return acc > 0;
        }

        public static BigInteger256 operator +(in BigInteger256 left, in BigInteger256 right) {
            var res = new BigInteger256(left);
            res.AssignAdd(right);
            return res;
        }
    }
}