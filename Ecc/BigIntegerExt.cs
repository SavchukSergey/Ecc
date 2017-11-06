using System;
using System.Numerics;
using System.Text;

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

        public static BigInteger ModMul(this BigInteger a, BigInteger b, BigInteger modulus) {
            return (a * b) % modulus;
        }

        public static BigInteger ModDiv(this BigInteger a, BigInteger b, BigInteger modulus) {
            return a.ModMul(b.ModInverse(modulus), modulus);
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

        public static BigInteger ModSqrt(this BigInteger val, BigInteger modulus) {
            var exp = (modulus + 1).ModDiv(4, modulus);
            return BigInteger.ModPow(val, exp, modulus);
        }

        public static string ToHexUnsigned(this BigInteger val, long length) {
            var sbLength = (int)length * 2;
            var sb = new StringBuilder(sbLength, sbLength);
            var data = val.ToByteArray();
            var dataLength = data.Length;
            const string hex = "0123456789abcdef";
            for (var i = length - 1; i >= 0; i--) {
                if (i < dataLength) {
                    var ch = data[i];
                    sb.Append(hex[ch >> 4]);
                    sb.Append(hex[ch & 0x0f]);
                } else {
                    sb.Append("00");
                }
            }
            return sb.ToString();
        }

        public static BigInteger ParseHexUnsigned(string val) {
            if (val.StartsWith("0x")) val = "0" + val.Substring(2);
            else val = "00" + val;
            return BigInteger.Parse(val, System.Globalization.NumberStyles.AllowHexSpecifier);
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
