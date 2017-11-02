using System.Numerics;

namespace Ecc {
    public static class BigIntegerExt {

        public static BigInteger ModAbs(this BigInteger val, BigInteger modulus) {
            if (val < 0) return modulus - ((-val) % modulus);
            return val % modulus;
        }

        public static BigInteger ModInverse(this BigInteger val, BigInteger modulus) {
            return EuclidExtended(val.ModAbs(modulus), modulus).X.ModAbs(modulus);
        }

        public static bool ModEqual(BigInteger a, BigInteger b, BigInteger modulus) {
            return (a % modulus) == (b % modulus);
        }

        public static BezoutIdentity EuclidExtended(BigInteger a, BigInteger b) {
            var s0 = new BigInteger(1);
            var t0 = new BigInteger(0);
            var s1 = new BigInteger(0);
            var t1 = new BigInteger(1);
            var r0 = a;
            var r1 = b;

            while (r1 != 0) {
                var quotient = BigInteger.DivRem(r0, r1, out BigInteger r2);
                var s2 = s0 - quotient * s1;
                var t2 = t0 - quotient * t1;
                s0 = s1;
                s1 = s2;
                t0 = t1;
                t1 = t2;
                r0 = r1;
                r1 = r2;
            }
            return new BezoutIdentity {
                A = a,
                B = b,
                X = s0,
                Y = t0
            };
        }

    }
}
