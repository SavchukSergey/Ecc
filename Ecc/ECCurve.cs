using System;
using System.Collections.Generic;
using System.Numerics;

namespace Ecc {
    public class ECCurve {

        public readonly string Name;

        public readonly BigInteger A;

        public readonly BigInteger B;

        public readonly BigInteger Modulus;

        public readonly BigInteger Order;

        public readonly BigInteger Cofactor;

        public readonly ECPoint G;

        public readonly int KeySize;

        public readonly int KeySize8;

        public readonly long OrderSize;

        private readonly ECPointBitCache _cache;

        public ECCurve(string name,
            in BigInteger a, in BigInteger b,
            in BigInteger modulus, in BigInteger order,
            in BigInteger cofactor, in BigInteger gx, in BigInteger gy) {
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
            _cache = new ECPointBitCache(G, KeySize);
        }

        public bool Singular {
            get {
                return ((4 * A * A * A + 27 * B * B) % Modulus) == 0;
            }
        }

        public bool Has(in ECPoint p) {
            if (p.IsInfinity) return true;

            var left = p.Y * p.Y;
            var right = p.X * p.X * p.X + A * p.X + B;
            return BigIntegerExt.ModEqual(left, right, Modulus);
        }

        public ECPublicKey GetPublicKey(in BigInteger k) {
            var acc = ECPoint.Infinity;
            Span<byte> data = stackalloc byte[k.GetByteCount()];
            k.TryWriteBytes(data, out var _);
            var exp = 0;
            for (var i = 0; i < data.Length; i++) {
                var bt = data[i];
                for (var bit = 1; bit < 256; bit <<= 1) {
                    if ((bt & bit) != 0) {
                        acc += _cache.Pow2(exp);
                    }
                    exp++;
                }
            }
            return new ECPublicKey(acc);
        }

        public ECPoint CreatePoint(in BigInteger x, in BigInteger y) {
            return new ECPoint(x, y, this);
        }

        public ECPoint CreatePoint(in BigInteger x, bool yOdd) {
            var right = x * x * x + A * x + B;
            var y = right.ModSqrt(Modulus);
            return CreatePoint(x, y);
        }

        public ECPoint CreatePoint(string hex) {
            if (hex == "00") return ECPoint.Infinity;
            if (hex.StartsWith("02")) {
                var x = BigIntegerExt.ParseHexUnsigned(hex.Substring(2));
                return CreatePoint(x, false);
            } else if (hex.StartsWith("03")) {
                var x = BigIntegerExt.ParseHexUnsigned(hex.Substring(2));
                return CreatePoint(x, true);
            } else if (hex.StartsWith("04")) {
                var keySize = KeySize8 * 2;
                var x = BigIntegerExt.ParseHexUnsigned(hex.Substring(2, keySize));
                var y = BigIntegerExt.ParseHexUnsigned(hex.Substring(keySize + 2));
                return CreatePoint(x, y);
            }
            throw new System.FormatException();
        }

        public ECPrivateKey CreateKeyPair() {
            return ECPrivateKey.Create(this);
        }

        public ECPrivateKey CreatePrivateKey(string hex) {
            return ECPrivateKey.ParseHex(hex, this);
        }

        public ECPrivateKey CreatePrivateKey(ReadOnlySpan<byte> data) {
            return new ECPrivateKey(BigIntegerExt.FromBigEndianBytes(data), this);
        }

        public ECPrivateKey CreatePrivateKey(in BigInteger data) {
            return new ECPrivateKey(data, this);
        }

        public ECPublicKey CreatePublicKey(string hex) {
            var point = CreatePoint(hex);
            return new ECPublicKey(point);
        }

        public BigInteger TruncateHash(ReadOnlySpan<byte> hash) {
            return TruncateHash(hash, BigIntegerExt.FromBigEndianBytes(hash));
        }

        public BigInteger TruncateHash(in BigInteger hash) {
            var bytesCount = hash.GetBigEndianBytesCount();
            Span<byte> data = stackalloc byte[bytesCount];
            hash.ToBigEndianBytes(data);
            return TruncateHash(data, hash);
        }

        private BigInteger TruncateHash(ReadOnlySpan<byte> data, BigInteger num) {
            var maxLength = OrderSize;
            var len = data.Length * 8;
            var extra = maxLength - len;
            if (extra > 0) {
                num >>= (int)extra;
            }
            return num;
        }

        private static ECCurve? _secp256k1;
        public static ECCurve Secp256k1 {
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
                }.Build();
            }
        }

        public static ECCurve Secp384r1 {
            get {
                return new ECHexInfo {
                    Name = "secp384r1",
                    P = "0xfffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffeffffffff0000000000000000ffffffff",
                    A = "0xfffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffeffffffff0000000000000000fffffffc",
                    B = "0xb3312fa7e23ee7e4988e056be3f82d19181d9c6efe8141120314088f5013875ac656398d8a2ed19d2a85c8edd3ec2aef",
                    Gx = "0xaa87ca22be8b05378eb1c71ef320ad746e1d3b628ba79b9859f741e082542a385502f25dbf55296c3a545e3872760ab7",
                    Gy = "0x3617de4a96262c6f5d9e98bf9292dc29f8f41dbd289a147ce9da3113b5f0b8c00a60b1ce1d7e819d7a431d7c90ea0e5f",
                    N = "0xffffffffffffffffffffffffffffffffffffffffffffffffc7634d81f4372ddf581a0db248b0a77aecec196accc52973",
                    H = "0x1"
                }.Build();
            }
        }

        public static ECCurve Secp521r1 {
            get {
                return new ECHexInfo {
                    Name = "secp521r1",
                    P = "0x01ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff",
                    A = "0x01fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffc",
                    B = "0x0051953eb9618e1c9a1f929a21a0b68540eea2da725b99b315f3b8b489918ef109e156193951ec7e937b1652c0bd3bb1bf073573df883d2c34f1ef451fd46b503f00",
                    Gx = "0x00c6858e06b70404e9cd9e3ecb662395b4429c648139053fb521f828af606b4d3dbaa14b5e77efe75928fe1dc127a2ffa8de3348b3c1856a429bf97e7e31c2e5bd66",
                    Gy = "0x011839296a789a3bc0045c8a5fb42c7d1bd998f54449579b446817afbd17273e662c97ee72995ef42640c550b9013fad0761353c7086a272c24088be94769fd16650",
                    N = "0x01fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffa51868783bf2f966b7fcc0148f709a5d03bb5c9b8899c47aebb6fb71e91386409",
                    H = "0x1"
                }.Build();
            }
        }

        public static ECCurve NistP256 {
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
                }.Build();
            }
        }

        public static ECCurve GetNamedCurve(string name) {
            return name switch {
                "secp256k1" => Secp256k1,
                "secp384r1" => Secp384r1,
                "secp521r1" => Secp521r1,
                "nistP256" => NistP256,
                _ => throw new Exception($"unknown curve: {name}"),
            };
        }

        public static IEnumerable<ECCurve> GetNamedCurves() {
            yield return Secp256k1;
            yield return Secp384r1;
            yield return Secp521r1;
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
