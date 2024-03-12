using System;
using System.Security.Cryptography;

namespace Ecc.Math {
    public static class BigInteger256Ext {

        private static readonly RandomNumberGenerator _cng = RandomNumberGenerator.Create();

        public static BigInteger256 ModDiv(this in BigInteger256 a, in BigInteger256 b, in BigInteger256 modulus) {
            return a.ModMul(b.ModInverse(modulus), modulus);
        }

        public static BigInteger256 FromBigEndianBytes(byte[] data) {
            Span<byte> reverse = stackalloc byte[BigInteger256.BYTES_SIZE];
            var len = System.Math.Min(data.Length, BigInteger256.BYTES_SIZE);
            var ptr = data.Length - 1;
            for (var i = 0; i < len; i++) {
                reverse[i] = data[ptr--];
            }
            for (var i = len; i < BigInteger256.BYTES_SIZE; i++) {
                reverse[i] = 0;
            }
            return new BigInteger256(reverse);
        }

        public static byte[] ToBigEndianBytes(this in BigInteger256 val) {
            var ptr = BigInteger256.BYTES_SIZE - 1;
            var reverse = new byte[BigInteger256.BYTES_SIZE];
            for (var i = 0; i < BigInteger256.BYTES_SIZE; i++) {
                reverse[i] = val.GetByte(ptr--);
            }
            return reverse;
        }

        public static BigInteger256 ModSqrt(this in BigInteger256 val, in BigInteger256 modulus) {
            var exp = modulus.ModAdd(new BigInteger256(1), modulus).ModDiv(new BigInteger256(4), modulus);
            return val.ModPow(exp, modulus);
        }

        public static BigInteger256 ModRandom(in BigInteger256 modulus) {
            Span<byte> data = stackalloc byte[BigInteger256.BYTES_SIZE];

            for (var i = 0; i < 1000; i++) {
                _cng.GetBytes(data);
                var walker = new BigInteger256(data);
                if (BigInteger256.Compare(walker, modulus) == -1) {
                    return walker;
                }
            }
            throw new Exception("Unable to generate random");
        }

        public static BigInteger256 ModRandomNonZero(in BigInteger256 modulus) {
            for (var i = 0; i < 1000; i++) {
                var rnd = ModRandom(modulus);
                if (!rnd.IsZero) {
                    return rnd;
                }
            }
            throw new Exception("Unable to generate random");
        }

        public static string ToBase64UrlUnsigned(this in BigInteger256 val, long length) {
            var data = val.ToBigEndianBytes();
            return Base64Url.Encode(data, data.Length - length, length);
        }

        public static BigInteger256 ParseBase64UrlUnsigned(string val) {
            var data = Base64Url.Decode(val);
            return FromBigEndianBytes(data);
        }

        public static BezoutIdentity256 EuclidExtended(in BigInteger256 a, in BigInteger256 b) {
            var s0 = new BigInteger256(1);
            var t0 = new BigInteger256(0);
            var s1 = new BigInteger256(0);
            var t1 = new BigInteger256(1);
            var r0 = a;
            var r1 = b;

            //https://stackoverflow.com/questions/67097428/is-it-possible-to-implement-the-extended-euclidean-algorithm-with-unsigned-machi
            var cnt = false;
            while (!r1.IsZero) {
                var quotient = BigInteger256.DivRem(in r0, in r1, out var r2);
                var s2 = s0 + BigInteger256.MulLow(in quotient, in s1);
                var t2 = t0 + BigInteger256.MulLow(in quotient, in t1);

                s0 = s1;
                s1 = s2;
                t0 = t1;
                t1 = t2;
                r0 = r1;
                r1 = r2;

                cnt = !cnt;
            }
            if (cnt) {
                s0 = b - s0;
            } else {
                t0 = a - t0;
            }
            return new BezoutIdentity256 {
                A = a,
                B = b,
                X = s0,
                Y = t0
            };
        }

    }
}
