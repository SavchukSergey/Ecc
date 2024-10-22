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

    }
}