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
            bool carry = false;
            for (var i = 0; i < UINT32_SIZE; i++) {
                ulong acc = UInt32[i];
                acc += i < BigInteger256.UINT32_SIZE ? other.UInt32[i] : 0; //todo: remove i < Itemssize
                acc += carry ? 1ul : 0ul;
                UInt32[i] = (uint)acc;
                carry = acc > uint.MaxValue;
            }
            return carry;
        }
    }
}