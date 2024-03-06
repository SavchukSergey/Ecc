using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Ecc {
    public static class BigIntegerExt {

        private static RandomNumberGenerator _cng = RandomNumberGenerator.Create();

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

        public static BigInteger FromBigEndianBytes(byte[] data) {
            var len = data.Length;
            var reverse = new byte[len + 1];
            for (var i = 0; i < len; i++) {
                reverse[i] = data[len - i - 1];
            }
            return new BigInteger(reverse);
        }

        public static byte[] ToBigEndianBytes(this in BigInteger val) {
            var data = val.ToByteArray();
            var len = data.Length;
            var reverse = new byte[len];
            for (var i = 0; i < len; i++) {
                reverse[i] = data[len - i - 1];
            }
            return reverse;
        }

        public static byte[] ToBigEndianBytes(this in BigInteger val, byte[] data, int offset, int length) {
            var src = val.ToByteArray();
            var actualLength = System.Math.Min(src.Length, length);
            for (var i = 0; i < actualLength; i++) {
                data[i + offset] = src[actualLength - i - 1];
            }
            for (var i = actualLength; i < length; i++) {
                data[i + offset] = 0;
            }
            return data;
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

        public static string ToBase64UrlUnsigned(this in BigInteger val, long length) {
            var data = val.ToBigEndianBytes();
            return Base64Url.Encode(data, data.Length - length, length);
        }

        public static BigInteger ParseBase64UrlUnsigned(string val) {
            var data = Base64Url.Decode(val);
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
