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

        public bool AssignAdd(in BigInteger256 other) {
            var carry = BiLow256.AssignAdd(other);
            if (carry) {
                HighUInt128++;
                if (HighUInt128 == 0) {
                    return true;
                }
            }
            return false;
        }

        public void AssignIncrement() {
            BiLow256.AssignIncrement();
            if (BiLow256.IsZero) {
                HighUInt128++;
            }
        }

        public static BigInteger384 operator +(in BigInteger384 left, in BigInteger384 right) {
            var res = new BigInteger384(left);
            res.AssignAdd(right);
            return res;
        }
    }
}