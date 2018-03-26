using System;
using System.Numerics;

namespace Ecc {
    public class ECPrivateKey {

        public readonly ECCurve Curve;

        public readonly BigInteger D;

        public ECPublicKey PublicKey => new ECPublicKey(Curve.G * D, Curve);

        public ECPrivateKey(BigInteger d, ECCurve curve) {
            if (d.Sign < 0) throw new ArgumentOutOfRangeException();
            D = d;
            Curve = curve;
        }

        public ECSignature Sign(byte[] hash) {
            var num = new BigInteger(hash); //todo: big-endian?
            return Sign(num);
        }

        public ECSignature Sign(BigInteger message) {
            message = Curve.TruncateHash(message);
            ECSignature signature = null;
            do {
                var random = BigIntegerExt.ModRandom(Curve.Order);
                signature = Sign(message, random);
            } while (signature == null);
            return signature;
        }

        public ECSignature Sign(BigInteger message, BigInteger random) {
            var p = Curve.G * random;
            var r = p.X % Curve.Order;
            if (r == 0) return null;
            var s = ((message + r * D) * BigIntegerExt.ModInverse(random, Curve.Order)) % Curve.Order;
            if (s == 0) return null;
            return new ECSignature {
                R = r,
                S = s
            };
        }

        public static ECPrivateKey Create(ECCurve curve) {
            var priv = BigIntegerExt.ModRandom(curve.Order);
            //todo: check not zero
            return new ECPrivateKey(priv, curve);
        }

        public static ECPrivateKey ParseHex(string hex, ECCurve curve) {
            return new ECPrivateKey(BigIntegerExt.ParseHexUnsigned(hex), curve);
        }

    }
}
