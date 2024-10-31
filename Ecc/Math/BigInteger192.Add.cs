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

        public bool AssignAdd(in BigInteger128 other) {
            UInt128 acc = LowUInt64;
            acc += other.LowUInt64;
            LowUInt64 = (ulong)acc;
            acc >>= 64;
            acc += MiddleUInt64;
            acc += other.HighUInt64;
            MiddleUInt64 = (ulong)acc;
            acc >>= 64;
            HighUInt64 += (ulong)acc;
            return (acc >> 64) != 0;
        }

        public static BigInteger192 operator +(in BigInteger192 left, in BigInteger192 right) {
            var res = new BigInteger192(left);
            res.AssignAdd(right);
            return res;
        }
    }
}