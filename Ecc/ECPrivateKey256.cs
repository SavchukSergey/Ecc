using System;
using System.ComponentModel;
using System.Numerics;
using Ecc.Math;

namespace Ecc {
    public readonly struct ECPrivateKey256 {

        public readonly ECCurve256 Curve;

        public readonly BigInteger256 D;

        public readonly ECPublicKey256 PublicKey => Curve.GetPublicKey(D);

        public ECPrivateKey256(in BigInteger256 d, ECCurve256 curve) {
            D = d;
            Curve = curve;
        }

        public readonly ECSignature256 Sign(byte[] hash) {
            var num = BigIntegerExt.FromBigEndianBytes(hash);
            return Sign(num);
        }

        public readonly ECSignature256? Sign(byte[] hash, in BigInteger256 random) {
            var num = BigIntegerExt.FromBigEndianBytes(hash);
            return Sign(num, random);
        }

        public readonly ECSignature256 Sign(in BigInteger message) {
            var truncated = Curve.TruncateHash(message);
            ECSignature256? signature;
            do {
                var random = BigInteger256Ext.ModRandom(Curve.Order);
                signature = SignTruncated(truncated, random);
            } while (signature == null);
            return signature.Value;
        }

        public readonly ECSignature256? Sign(in BigInteger message, in BigInteger256 random) {
            var truncated = Curve.TruncateHash(message);
            return SignTruncated(truncated, random);
        }

        private readonly ECSignature256? SignTruncated(in BigInteger256 message, in BigInteger256 random) {
            var on = Curve.Order.ToNative();
            var p = Curve.G * random;
            var r = p.X % on;
            if (r == 0) return null;
            var s = (message.ToNative() + r * D.ToNative()) * BigInteger256Ext.ModInverse(random, Curve.Order).ToNative() % on;
            if (s == 0) return null;
            return new ECSignature256(new BigInteger256(r), new BigInteger256(s), Curve);
        }

        public static ECPrivateKey256 Create(ECCurve256 curve) {
            var priv = BigInteger256Ext.ModRandomNonZero(curve.Order);
            return new ECPrivateKey256(priv, curve);
        }

        public static ECPrivateKey256 ParseHex(string hex, ECCurve256 curve) {
            return new ECPrivateKey256(BigInteger256Ext.ParseHexUnsigned(hex), curve);
        }

    }
}
