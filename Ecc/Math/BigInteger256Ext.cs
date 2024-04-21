using System;
using System.Security.Cryptography;

namespace Ecc.Math {
    public static class BigInteger256Ext {

        private static readonly RandomNumberGenerator _cng = RandomNumberGenerator.Create();

        public static BigInteger256 ModDiv(this in BigInteger256 a, in BigInteger256 b, in BigInteger256 modulus) {
            return a.ModMul(b.ModInverse(modulus), modulus);
        }

        public static BigInteger256 ModDivPrime(this in BigInteger256 a, in BigInteger256 b, in BigInteger256 primeModulus) {
            return a.ModMul(b.ModInversePrime(primeModulus), primeModulus);
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

        public static BigInteger256 ModSqrtPrime(this in BigInteger256 val, in BigInteger256 primeModulus) {
            var exp = primeModulus.ModAdd(new BigInteger256(1), primeModulus).ModDivPrime(new BigInteger256(4), primeModulus);
            return val.ModPow(exp, primeModulus);
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



    }
}
