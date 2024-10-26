using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger256 operator -(in BigInteger256 left, in BigInteger256 right) {
            var res = left;
            var borrow = left.LowUInt128 < right.LowUInt128;
            res.LowUInt128 -= right.LowUInt128;
            res.HighUInt128 -= right.HighUInt128;
            if (borrow) {
                res.HighUInt128--;
            }
            return res;
        }

        public readonly BigInteger256 Sub(in BigInteger256 other, out bool carry) {
            carry = false;
            var res = new BigInteger256(this);
            for (var i = 0; i < UINT64_SIZE; i++) {
                UInt128 acc = UInt64[i];
                acc -= other.UInt64[i];
                acc -= carry ? 1u : 0u;
                res.UInt64[i] = (ulong)acc;
                carry = acc > ulong.MaxValue; //todo: use shift to avoid branching
            }
            return res;
        }

        public void AssignSub(in BigInteger256 other) {
            var borrow = LowUInt128 < other.LowUInt128;
            LowUInt128 -= other.LowUInt128;
            HighUInt128 -= other.HighUInt128;
            if (borrow) {
                HighUInt128--;
            }
        }

        public void AssignSub(in BigInteger192 other) {
            var borrow = LowUInt128 < other.LowUInt128;
            LowUInt128 -= other.LowUInt128;
            if (borrow) {
                HighUInt128--;
            }
            HighUInt128 -= other.HighUInt64;
        }

        public void AssignSub(in BigInteger128 other) {
            var borrow = LowUInt128 < other.UInt128;
            LowUInt128 -= other.UInt128;
            if (borrow) {
                HighUInt128--;
            }
        }

    }
}