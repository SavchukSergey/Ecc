using System.Numerics;
using Ecc.Math;

namespace Ecc {
    public readonly struct ECPublicKey256 {

        public readonly ECCurve256 Curve;

        public readonly ECPoint256 Point;

        public ECPublicKey256(in ECPoint256 point) {
            Point = point;
            Curve = point.Curve;
        }

        public bool VerifySignature(in BigInteger hash, in ECSignature256 signature) {
            var truncated = Curve.TruncateHash(hash);
            var on = Curve.Order.ToNative();
            var w = signature.S.ModInverse(Curve.Order);
            var u1 = new BigInteger256(truncated.ToNative() * w.ToNative() % on);
            var rn = signature.R.ToNative();
            var u2 = new BigInteger256(rn * w.ToNative() % on);
            var p = Curve.G * u1 + Point * u2;
            return BigIntegerExt.ModEqual(rn, p.X, on);
        }

        public bool VerifySignature(byte[] hash, in ECSignature256 signature) {
            return VerifySignature(BigIntegerExt.FromBigEndianBytes(hash), signature);
        }

        public override string ToString() => Point.GetHex();

    }
}
