using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger384 {

        public bool AssignAdd(in BigInteger384 other) {
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

            acc += UInt64[4];
            acc += other.UInt64[4];
            UInt64[4] = (ulong)acc;
            acc >>= 64;

            acc += UInt64[5];
            acc += other.UInt64[5];
            UInt64[5] = (ulong)acc;
            acc >>= 64;

            return acc > 0;
        }

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

            acc += UInt64[4];
            UInt64[4] = (ulong)acc;
            acc >>= 64;

            acc += UInt64[5];
            UInt64[5] = (ulong)acc;
            acc >>= 64;

            return acc > 0;
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