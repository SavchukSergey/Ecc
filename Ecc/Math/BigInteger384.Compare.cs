using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger384 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(in BigInteger384 left, in BigInteger384 right) {
            if (left.HighUInt128 > right.HighUInt128) {
                return 1;
            }
            if (left.HighUInt128 < right.HighUInt128) {
                return -1;
            }
            if (left.BiLow256 > right.BiLow256) {
                return 1;
            }
            if (left.BiLow256 < right.BiLow256) {
                return -1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(in BigInteger384 left, in BigInteger192 right) {
            if (!left.BiHigh256.IsZero) {
                return 1;
            }

            return BigInteger192.Compare(left.BiLow192, right);
        }

        public static bool operator <(in BigInteger384 left, in BigInteger384 right) {
            return Compare(left, right) < 0;
        }

        public static bool operator >(in BigInteger384 left, in BigInteger384 right) {
            return Compare(left, right) > 0;
        }

        public static bool operator <=(in BigInteger384 left, in BigInteger384 right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator >=(in BigInteger384 left, in BigInteger384 right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator ==(in BigInteger384 left, in BigInteger384 right) {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(in BigInteger384 left, in BigInteger384 right) {
            return Compare(left, right) != 0;
        }


        public static bool operator <(in BigInteger384 left, in BigInteger192 right) {
            return Compare(left, right) < 0;
        }

        public static bool operator >(in BigInteger384 left, in BigInteger192 right) {
            return Compare(left, right) > 0;
        }

        public static bool operator <=(in BigInteger384 left, in BigInteger192 right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator >=(in BigInteger384 left, in BigInteger192 right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator ==(in BigInteger384 left, in BigInteger192 right) {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(in BigInteger384 left, in BigInteger192 right) {
            return Compare(left, right) != 0;
        }

    }
}