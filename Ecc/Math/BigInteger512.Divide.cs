using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public static BigInteger256 operator %(in BigInteger512 left, in BigInteger256 right) {
            return new BigInteger256(left.ToNative() % right.ToNative());
        }

        public static BigInteger512 DivRem64(in BigInteger512 dividend, ulong divisor, out BigInteger512 remainder) {
            if (divisor <= uint.MaxValue) {
                return DivRem32(dividend, (uint)divisor, out remainder);
            }
            var quotient = new BigInteger512();
            var rem = new UInt128();
            for (var i = UINT64_SIZE - 1; i >= 0; i--) {
                var partialDividend = (rem << 64) + dividend.UInt64[i];
                var partialQuotient = partialDividend / divisor;
                var partialRemainder = partialDividend % divisor;
                quotient.UInt64[i] = (ulong)partialQuotient;
                rem = partialRemainder;
            }
            remainder = new BigInteger512(rem);

            return quotient;
        }

        public static BigInteger512 DivRem32(in BigInteger512 dividend, uint divisor, out BigInteger512 remainder) {
            var quotient = new BigInteger512();
            var rem = 0ul;
            for (var i = UINT32_SIZE - 1; i >= 0; i--) {
                var partialDividend = (rem << 32) + dividend.UInt32[i];
                var partialQuotient = partialDividend / divisor;
                var partialRemainder = partialDividend % divisor;
                quotient.UInt32[i] = (uint)partialQuotient;
                rem = partialRemainder;
            }
            remainder = new BigInteger512(rem);

            return quotient;
        }
    }
}