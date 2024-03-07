using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Ecc.Math {
    public static class BigInteger256Ext {

        private static RandomNumberGenerator _cng = RandomNumberGenerator.Create();


        [Obsolete]
        public static BigInteger ModAbs(this in BigInteger val, in BigInteger modulus) {
            if (val.Sign == -1) {
                return modulus - ((-val) % modulus);
            }
            if (val < modulus) {
                return val;
            }
            return val % modulus;
        }

        public static BigInteger256 ModDiv(this in BigInteger256 a, in BigInteger256 b, in BigInteger256 modulus) {
            return a.ModMul(b.ModInverse(modulus), modulus);
        }

        public static long Log2(this in BigInteger256 val) {
            var res = BigInteger256.BITS_SIZE;
            for (var i = BigInteger256.ITEMS_SIZE - 1; i >= 0; i--) {
                var item = val.GetItem(i);
                var mask = 0x8000_0000;
                while (mask != 0) {
                    if ((item & mask) != 0) {
                        return res;
                    }
                    mask >>= 1;
                    res--;
                }
            }
            return res;
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

        public static BigInteger256 ParseHexUnsigned(string val) {
            if (val.StartsWith("0x")) {
                val = val.Substring(2);
            }
            var res = new BigInteger256();
            res.ReadFromHex(val);
            return res;
        }

        public static BezoutIdentity256 EuclidExtended(in BigInteger256 a, in BigInteger256 b) {
            var s0 = new BigInteger256(1);
            var t0 = new BigInteger256(0);
            var s1 = new BigInteger256(0);
            var t1 = new BigInteger256(1);
            var r0 = a;
            var r1 = b;

            //todo: optimize

            var cnt = false;
            while (!r1.IsZero) {
                var quotient = BigInteger256.DivRem(r0, r1, out var r2);
                var qr1 = quotient * r1;
                var qs1 = quotient * s1;
                var qt1 = quotient * t1;

                r0 = r0 > new BigInteger256(qr1.Low) ? r2 : new BigInteger256(0) - r2;
                (r1, r0) = (r0, r1);
                s0 = s0 + qs1.Low;
                (s1, s0) = (s0, s1);
                t0 = t0 + qt1.Low;
                (t1, t0) = (t0, t1);
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