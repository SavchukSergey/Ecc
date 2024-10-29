using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public bool AssignAdd(in BigInteger512 other) {
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

        public bool AssignAdd(in BigInteger256 other) {
            var carry = Low.AssignAdd(other);
            if (carry) {
                return High.AssignIncrement();
            }
            return carry;
        }
    }
}