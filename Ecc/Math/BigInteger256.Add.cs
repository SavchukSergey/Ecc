using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public bool AssignAdd(in BigInteger256 other) {
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

        public void AssignAddMiddle(in BigInteger128 other) {
            var acc = (UInt128)other.LowUInt64 + UInt64[1];
            UInt64[1] = (ulong)acc;
            acc >>= 64;
            acc = (UInt128)other.HighUInt64 + UInt64[2] + acc;
            UInt64[2] = (ulong)acc;
            acc >>= 64;
            HighUInt64 += (ulong)acc;
        }

        public void AssignAddHigh(in BigInteger192 other) {
            var acc = (UInt128)other.LowUInt64 + UInt64[1];
            UInt64[1] = (ulong)acc;
            acc >>= 64;
            acc = (UInt128)other.MiddleUInt64 + UInt64[2] + acc;
            UInt64[2] = (ulong)acc;
            acc >>= 64;
            acc = (UInt128)other.HighUInt64 + UInt64[3] + acc;
            UInt64[3] = (ulong)acc;
        }

        public static BigInteger256 operator +(in BigInteger256 left, in BigInteger256 right) {
            var res = new BigInteger256(left);
            res.AssignAdd(right);
            return res;
        }
    }
}