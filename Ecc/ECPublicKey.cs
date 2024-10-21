using System;
using System.Numerics;

namespace Ecc {
    public readonly struct ECPublicKey {

        public readonly ECCurve Curve;

        public readonly ECPoint Point;

        public ECPublicKey(in ECPoint point) {
            Point = point;
            Curve = point.Curve;
        }

        public bool VerifySignature(in BigInteger hash, in ECSignature signature) {
            var truncated = Curve.TruncateHash(hash);
            var w = signature.S.ModInverse(Curve.Order);
            var u1 = (truncated * w) % Curve.Order;
            var u2 = (signature.R * w) % Curve.Order;
            var p = Curve.G * u1 + Point * u2;
            return BigIntegerExt.ModEqual(signature.R, p.X, Curve.Order);
        }

        public bool VerifySignature(ReadOnlySpan<byte> hash, in ECSignature signature) {
            return VerifySignature(BigIntegerExt.FromBigEndianBytes(hash), signature);
        }

        public override string ToString() => Point.GetHex();

    }
}
