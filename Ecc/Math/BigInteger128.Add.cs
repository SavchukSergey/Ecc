using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger128 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignAdd(in BigInteger128 other) {
            UInt128 += other.UInt128;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignAdd(in UInt128 other) {
            UInt128 += other;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignAddHigh(ulong other) {
            High += other;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger128 operator +(in BigInteger128 left, in BigInteger128 right) {
            return new BigInteger128(left.UInt128 + right.UInt128);
        }
    }
}