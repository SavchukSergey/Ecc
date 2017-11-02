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

        public static long Log2(this BigInteger val) {
            var n = val.ToByteArray();
            var keySize = n.Length * 8;
            for (var i = n.Length - 1; i >= 0; i--) {
                var bt = n[i];
                for (var j = 7; j >= 0; j--) {
                    if ((bt & (1 << j)) != 0) {
                        return keySize;
                    }
                    keySize--;
                }
            }
            return keySize;
        }

        public static string ToHexUnsigned(this BigInteger val) {
            var res = val.ToString("x");
            if (res.Length % 2 == 0) return res;
            if (res.StartsWith("0")) return res.Substring(1);
            return "f" + res;
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
