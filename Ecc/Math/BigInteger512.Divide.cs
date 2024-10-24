using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public static BigInteger256 operator %(in BigInteger512 left, in BigInteger256 right) {
            DivRemGuess(left, right, out BigInteger256 remainder);
            return remainder;
        }

        public static BigInteger512 DivRem(in BigInteger512 dividend, in BigInteger512 divisor, out BigInteger512 remainder) {
            return DivRemGuess(dividend, divisor, out remainder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger512 DivRem64(in BigInteger512 dividend, ulong divisor, out BigInteger512 remainder) {
            var res = DivRem64(in dividend, divisor, out ulong remainder64);
            remainder = new BigInteger512(remainder64);
            return res;
        }

        public static BigInteger512 DivRem64(in BigInteger512 dividend, ulong divisor, out ulong remainder) {
            if (divisor <= uint.MaxValue) {
                var res = DivRem32(dividend, (uint)divisor, out uint remainder32);
                remainder = remainder32;
                return res;
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

            remainder = (ulong)rem;

            return quotient;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger512 DivRem32(in BigInteger512 dividend, uint divisor, out BigInteger512 remainder) {
            var res = DivRem32(in dividend, divisor, out uint remainder32);
            remainder = new BigInteger512(remainder32);
            return res;
        }

        public static BigInteger512 DivRem32(in BigInteger512 dividend, uint divisor, out uint remainder) {
            var quotient = new BigInteger512();
            var rem = 0ul;
            for (var i = UINT32_SIZE - 1; i >= 0; i--) {
                var partialDividend = (rem << 32) + dividend.UInt32[i];
                var partialQuotient = partialDividend / divisor;
                var partialRemainder = partialDividend % divisor;
                quotient.UInt32[i] = (uint)partialQuotient;
                rem = partialRemainder;
            }
            remainder = (uint)rem;

            return quotient;
        }
    }
}