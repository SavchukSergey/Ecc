﻿using System;
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

        //todo: use out param instead on return
        public static BigInteger256 ModInverse(this in BigInteger256 val, in BigInteger256 modulus) {
            return new BigInteger256(
                EuclidExtended(val, modulus).X.ModAbs(modulus.ToNative())
            );
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

        public static byte[] ToBigEndianBytes(this in BigInteger256 val, byte[] data, int offset, int length) {
            var src = val.ToNative().ToByteArray();
            var actualLength = System.Math.Min(src.Length, length);
            for (var i = 0; i < actualLength; i++) {
                data[i + offset] = src[actualLength - i - 1];
            }
            for (var i = actualLength; i < length; i++) {
                data[i + offset] = 0;
            }
            return data;
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
            var an = a.ToNative();
            var bn = b.ToNative();
            var s0 = BigInteger.One;
            var t0 = BigInteger.Zero;
            var s1 = BigInteger.Zero;
            var t1 = BigInteger.One;
            var r0 = an;
            var r1 = bn;

            while (!r1.IsZero) {
                var quotient = BigInteger.DivRem(r0, r1, out var r2);
                var s2 = s0 - quotient * s1;
                var t2 = t0 - quotient * t1;
                s0 = s1;
                s1 = s2;
                t0 = t1;
                t1 = t2;
                r0 = r1;
                r1 = r2;
            }
            return new BezoutIdentity256 {
                A = an,
                B = bn,
                X = s0,
                Y = t0
            };
        }

    }
}
