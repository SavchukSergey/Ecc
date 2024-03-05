using System;
using System.Collections.Generic;
using System.Numerics;
using Ecc.Math;

namespace Ecc {
    public class ECCurve256 {

        public readonly string Name;

        public readonly BigInteger A;

        public readonly BigInteger B;

        public readonly BigInteger256 Modulus;

        public readonly BigInteger256 Order;

        public readonly BigInteger Cofactor;

        public readonly ECPoint256 G;

        public readonly bool Singluar;

        public readonly int KeySize;

        public readonly int KeySize8;

        public readonly long OrderSize;

        private readonly ECPointByteCache256 _cache;

        public ECCurve256(string name,
            in BigInteger a, in BigInteger b,
            in BigInteger256 modulus, in BigInteger256 order,
            in BigInteger cofactor, in BigInteger256 gx, in BigInteger256 gy) {
            Name = name;
            A = a;
            B = b;
            Modulus = modulus;
            Order = order;
            Cofactor = cofactor;
            G = CreatePoint(gx, gy);
            OrderSize = Order.Log2();
            KeySize = (int)Modulus.Log2();
            KeySize8 = (KeySize + 7) >> 3;
            Singluar = ((4 * A * A * A + 27 * B * B) % Modulus) == 0;
            _cache = new ECPointByteCache256(G, KeySize);
        }

        public bool Has(in ECPoint256 p) {
            if (p.IsInfinity) return true;

            var left = p.Y * p.Y;
            var right = p.X * p.X * p.X + A * p.X + B;
            return BigInteger256Ext.ModEqual(new BigInteger256(left), new BigInteger256(right), Modulus);
        }

        public ECPublicKey256 GetPublicKey(in BigInteger256 k) {
            var acc = ECPoint256.Infinity;
            for (var i = 0; i < BigInteger256.BYTES_SIZE; i++) {
                var bt = k.GetByte(i);
                acc += _cache.Get(i, bt);
            }
            return new ECPublicKey256(acc);
        }

        public ECPoint256 CreatePoint(in BigInteger256 x, in BigInteger256 y) {
            return new ECPoint256(x, y, this);
        }

        public ECPoint256 CreatePoint(in BigInteger256 x, bool yOdd) {
            var xn = x.ToNative();
            var right = xn * xn * xn + A * xn + B;
            var y = right.ModSqrt(Modulus.ToNative());
            return CreatePoint(x, new BigInteger256(y));
        }

        public ECPoint256 CreatePoint(string hex) {
            if (hex == "00") return ECPoint256.Infinity;
            if (hex.StartsWith("02")) {
                var x = BigInteger256Ext.ParseHexUnsigned(hex.Substring(2));
                return CreatePoint(x, false);
            } else if (hex.StartsWith("03")) {
                var x = BigInteger256Ext.ParseHexUnsigned(hex.Substring(2));
                return CreatePoint(x, true);
            } else if (hex.StartsWith("04")) {
                var keySize = KeySize8 * 2;
                var x = BigInteger256Ext.ParseHexUnsigned(hex.Substring(2, keySize));
                var y = BigInteger256Ext.ParseHexUnsigned(hex.Substring(keySize + 2));
                return CreatePoint(x, y);
            }
            throw new System.FormatException();
        }

        public ECPrivateKey256 CreateKeyPair() {
            return ECPrivateKey256.Create(this);
        }

        public ECPrivateKey256 CreatePrivateKey(string hex) {
            return ECPrivateKey256.ParseHex(hex, this);
        }

        public ECPrivateKey256 CreatePrivateKey(byte[] data) {
            return new ECPrivateKey256(BigInteger256Ext.FromBigEndianBytes(data), this);
        }

        public ECPrivateKey256 CreatePrivateKey(in BigInteger256 data) {
            return new ECPrivateKey256(data, this);
        }

        public ECPublicKey256 CreatePublicKey(string hex) {
            var point = CreatePoint(hex);
            return new ECPublicKey256(point);
        }

        public BigInteger256 TruncateHash(byte[] hash) {
            return TruncateHash(hash, BigIntegerExt.FromBigEndianBytes(hash));
        }

        public BigInteger256 TruncateHash(in BigInteger hash) {
            var arr = hash.ToBigEndianBytes();
            return TruncateHash(arr, hash);
        }

        private BigInteger256 TruncateHash(byte[] data, BigInteger num) {
            var maxLength = OrderSize;
            var len = data.Length * 8;
            var extra = maxLength - len;
            if (extra > 0) {
                num >>= (int)extra;
            }
            return new BigInteger256(num);
        }

        private static ECCurve256? _secp256k1;
        public static ECCurve256 Secp256k1 {
            get {
                return _secp256k1 ??= new ECHexInfo {
                    Name = "secp256k1",
                    P = "0xfffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f",
                    A = "0x0",
                    B = "0x7",
                    Gx = "0x79be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798",
                    Gy = "0x483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8",
                    N = "0xfffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141",
                    H = "0x1"
                }.Build256();
            }
        }

        public static ECCurve256 NistP256 {
            get {
                return new ECHexInfo {
                    Name = "nistP256",
                    P = "0xffffffff00000001000000000000000000000000ffffffffffffffffffffffff",
                    A = "0xffffffff00000001000000000000000000000000fffffffffffffffffffffffc",
                    B = "0x5ac635d8aa3a93e7b3ebbd55769886bc651d06b0cc53b0f63bce3c3e27d2604b",
                    Gx = "0x6b17d1f2e12c4247f8bce6e563a440f277037d812deb33a0f4a13945d898c296",
                    Gy = "0x4fe342e2fe1a7f9b8ee7eb4a7c0f9e162bce33576b315ececbb6406837bf51f5",
                    N = "0xffffffff00000000ffffffffffffffffbce6faada7179e84f3b9cac2fc632551",
                    H = "0x1"
                }.Build256();
            }
        }

        public static ECCurve256 GetNamedCurve(string name) {
            return name switch {
                "secp256k1" => Secp256k1,
                "nistP256" => NistP256,
                _ => throw new Exception($"unknown curve: {name}"),
            };
        }

        public static IEnumerable<ECCurve256> GetNamedCurves() {
            yield return Secp256k1;
            yield return NistP256;
        }

        public override int GetHashCode() {
            if (!string.IsNullOrWhiteSpace(Name)) {
                return Name.GetHashCode();
            }
            return Modulus.GetHashCode() ^ A.GetHashCode() ^ B.GetHashCode() ^ G.X.GetHashCode() ^ G.Y.GetHashCode() ^ Order.GetHashCode() ^ Cofactor.GetHashCode();
        }
    }
}
