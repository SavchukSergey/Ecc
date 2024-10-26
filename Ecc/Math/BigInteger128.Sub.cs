using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger128 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger128 operator -(in BigInteger128 left, in BigInteger128 right) {
            return new BigInteger128(left.UInt128 - right.UInt128);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignSub(in BigInteger128 other) {
            UInt128 -= other.UInt128;
        }

    }
}