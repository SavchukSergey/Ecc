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

        public readonly bool VerifySignature(in BigInteger hash, in ECSignature256 signature) {
            var truncated = Curve.TruncateHash(hash);
            var w = signature.S.ModInverse(Curve.Order);
            var u1 = truncated.ModMul(w, Curve.Order);
            var u2 = signature.R.ModMul(w, Curve.Order);
            var p = Curve.G * u1 + Point * u2;
            return signature.R.Equals(p.X);
        }

        public readonly bool VerifySignature(byte[] hash, in ECSignature256 signature) {
            return VerifySignature(BigIntegerExt.FromBigEndianBytes(hash), signature);
        }

        public override string ToString() => Point.GetHex();

    }
}
