using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger128 {

        public static BigInteger128 DivRem(in BigInteger128 dividend, in BigInteger128 divisor, out BigInteger128 remainder) {
            var res = new BigInteger128(dividend.UInt128 / divisor.UInt128);
            remainder = new BigInteger128(dividend.UInt128 % divisor.UInt128);
            return res;
        }

        public static BigInteger128 DivRem(in BigInteger128 dividend, ulong divisor, out ulong remainder) {
            var res = new BigInteger128(dividend.UInt128 / divisor);
            remainder = (ulong)(dividend.UInt128 % divisor);
            return res;
        }

        public static BigInteger128 DivRem(in BigInteger128 dividend, ulong divisor, out BigInteger128 remainder) {
            var res = new BigInteger128(dividend.UInt128 / divisor);
            remainder = new BigInteger128(dividend.UInt128 % divisor);
            return res;
        }

   }
}