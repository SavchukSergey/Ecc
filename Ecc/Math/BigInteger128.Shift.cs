using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger128 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger128 operator <<(in BigInteger128 value, int count) {
            return new BigInteger128(value.UInt128 << count);
        }

    }
}