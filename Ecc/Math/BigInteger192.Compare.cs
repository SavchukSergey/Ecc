using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(in BigInteger192 left, in BigInteger192 right) {
            if (left.HighUInt128 > right.HighUInt128) {
                return 1;
            }
            if (left.HighUInt128 < right.HighUInt128) {
                return -1;
            }
            if (left.LowUInt64 > right.LowUInt64) {
                return 1;
            }
            if (left.LowUInt64 < right.LowUInt64) {
                return -1;
            }
            return 0;
        }

        public static bool operator <(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) < 0;
        }

        public static bool operator >(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) > 0;
        }

        public static bool operator <=(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator >=(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator ==(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) != 0;
        }

    }
}