using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Ecc {
    public static class BigIntegerExt {

        private static readonly RandomNumberGenerator _cng = RandomNumberGenerator.Create();

        public static BigInteger ModAbs(this in BigInteger val, in BigInteger modulus) {
            if (val.Sign == -1) {
                return modulus - ((-val) % modulus);
            }
            if (val < modulus) {
                return val;
            }
            return val % modulus;
        }

        public static BigInteger ModInverse(this in BigInteger val, in BigInteger modulus) {
            return EuclidExtended(val.ModAbs(modulus), modulus).X.ModAbs(modulus);
        }

        public static bool ModEqual(in BigInteger a, in BigInteger b, in BigInteger modulus) {
            if (a.Sign == 1 && b.Sign == 1 && a < modulus & b < modulus) {
                return a == b;
            }
            return (a % modulus) == (b % modulus);
        }

        public static BigInteger ModMul(this in BigInteger a, in BigInteger b, in BigInteger modulus) {
            return (a * b) % modulus;
        }

        public static BigInteger ModDiv(this in BigInteger a, in BigInteger b, in BigInteger modulus) {
            return a.ModMul(b.ModInverse(modulus), modulus);
        }

        public static long Log2(this in BigInteger val) {
            Span<byte> bytes = stackalloc byte[GetBigEndianBytesCount(val)];
            val.ToBigEndianBytes(bytes);
            var keySize = bytes.Length * 8;
            for (var i = 0; i < bytes.Length; i++) {
                var bt = bytes[i];
                for (var j = 7; j >= 0; j--) {
                    if ((bt & (1 << j)) != 0) {
                        return keySize;
                    }
                    keySize--;
                }
            }
            return keySize;
        }

        public static BigInteger FromBigEndianBytes(ReadOnlySpan<byte> data) {
            return new BigInteger(data, isUnsigned: true, isBigEndian: true);
        }

        public static int GetBigEndianBytesCount(this in BigInteger val) {
            return val.GetByteCount(isUnsigned: true);
        }

        public static bool ToBigEndianBytes(this in BigInteger val, Span<byte> bytes) {
            return val.TryWriteBytes(bytes, out var _, isUnsigned: true, isBigEndian: true);
        }

        public static void ToBigEndianBytes(this in BigInteger val, Span<byte> data, int length) {
            val.ToBigEndianBytes(data);
            var actualLength = System.Math.Min(val.GetBigEndianBytesCount(), length);
            for (var i = actualLength; i < length; i++) {
                data[i] = 0;
            }
        }

        public static BigInteger ModSqrt(this in BigInteger val, in BigInteger modulus) {
            var exp = (modulus + 1).ModDiv(4, modulus);
            return BigInteger.ModPow(val, exp, modulus);
        }

        public static BigInteger ModRandom(in BigInteger modulus) {
            var size = modulus.GetByteCount(isUnsigned: true);
            Span<byte> data = stackalloc byte[size];
            Span<byte> modulusData = stackalloc byte[size];
            modulus.TryWriteBytes(modulusData, out var _, isUnsigned: true);

            var m = new BigRefInteger {
                Data = modulusData
            };

            var walker = new BigRefInteger {
                Data = data
            };

            for (var i = 0; i < 1000; i++) {
                _cng.GetBytes(data);
                if (BigRefInteger.Compare(walker, m) == -1) {
                    return walker.ToBigInteger();
                }
            }
            throw new Exception("Unable to generate random");
        }

        public static BigInteger ModRandomNonZero(in BigInteger modulus) {
            for (var i = 0; i < 1000; i++) {
                var rnd = ModRandom(modulus);
                if (!rnd.IsZero) {
                    return rnd;
                }
            }
            throw new Exception("Unable to generate random");
        }

        public static string ToHexUnsigned(this in BigInteger val, long length) {
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

        public static string ToBase64UrlUnsigned(this in BigInteger val, int length) {
            Span<byte> data = stackalloc byte[val.GetBigEndianBytesCount()];
            val.ToBigEndianBytes(data);
            return Base64Url.Encode(data.Slice(data.Length - length, length));
        }

        public static BigInteger ParseBase64UrlUnsigned(string val) {
            Span<byte> data = stackalloc byte[Base64Url.GetByteCount(val)];
            Base64Url.Decode(val, data);
            return FromBigEndianBytes(data);
        }

        public static BigInteger ParseHexUnsigned(string val) {
            if (val.StartsWith("0x")) val = "0" + val.Substring(2);
            else val = "00" + val;
            return BigInteger.Parse(val, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public static BezoutIdentity EuclidExtended(in BigInteger a, in BigInteger b) {
            var s0 = BigInteger.One;
            var t0 = BigInteger.Zero;
            var s1 = BigInteger.Zero;
            var t1 = BigInteger.One;
            var r0 = a;
            var r1 = b;

            while (!r1.IsZero) {
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
