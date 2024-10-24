using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger128 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger128 operator <<(in BigInteger128 value, int count) {
            return new BigInteger128(value.UInt128 << count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignLeftShift(int count) {
            UInt128 <<= count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignLeftShift8() {
            UInt128 <<= 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignLeftShift16() {
            UInt128 <<= 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignLeftShift32() {
            UInt128 <<= 32;
        }

    }
}