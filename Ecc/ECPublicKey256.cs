using System;
using Ecc.Math;

namespace Ecc {
    public readonly struct ECPublicKey256 {

        public readonly ECCurve256 Curve;

        public readonly ECPoint256 Point;

        public ECPublicKey256(in ECPoint256 point) {
            Point = point;
            Curve = point.Curve;
        }

        public readonly bool VerifySignature(in BigInteger256 hash, in ECSignature256 signature) {
            var w = signature.S.ModInverse(Curve.Order);
            var u1 = hash.ModMul(w, Curve.Order);
            var u2 = signature.R.ModMul(w, Curve.Order);
            var p = Curve.G * u1 + Point * u2;
            return signature.R.Equals(p.X);
        }

        public readonly bool VerifySignature(ReadOnlySpan<byte> hash, in ECSignature256 signature) {
            var truncated = Curve.TruncateHash(hash);
            return VerifySignature(in truncated, in signature);
        }

        public override string ToString() => Point.GetHex();

    }
}
