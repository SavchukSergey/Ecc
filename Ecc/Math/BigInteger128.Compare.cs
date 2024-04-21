using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger128 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(in BigInteger128 left, in BigInteger128 right) {
            return left.UInt128 < right.UInt128;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(in BigInteger128 left, in BigInteger128 right) {
            return left.UInt128 > right.UInt128;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(in BigInteger128 left, in BigInteger128 right) {
            return left.UInt128 <= right.UInt128;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(in BigInteger128 left, in BigInteger128 right) {
            return left.UInt128 >= right.UInt128;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(in BigInteger128 left, in BigInteger128 right) {
            if (left.UInt128 > right.UInt128) {
                return 1;
            }
            if (left.UInt128 < right.UInt128) {
                return -1;
            }

            return 0;
        }
    }
}