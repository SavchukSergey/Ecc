using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger256 operator -(in BigInteger256 left, in BigInteger256 right) {
            var res = left;
            var borrow = left.Low < right.Low;
            res.Low -= right.Low;
            res.High -= right.High;
            if (borrow) {
                res.High--;
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
            var borrow = Low < other.Low;
            Low -= other.Low;
            High -= other.High;
            if (borrow) {
                High--;
            }
        }


    }
}