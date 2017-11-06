using System.Numerics;

namespace Ecc {
    public class ECPublicKey {

        public readonly ECCurve Curve;

        public readonly ECPoint Point;

        public ECPublicKey(ECPoint point, ECCurve curve) {
            Point = point;
            Curve = curve;
        }

        public bool VerifySignature(BigInteger message, ECSignature signature) {
            var w = signature.S.ModInverse(Curve.Order);
            var u1 = (message * w) % Curve.Order;
            var u2 = (signature.R * w) % Curve.Order;
            var p = Curve.G * u1 + Point * u2;
            return BigIntegerExt.ModEqual(signature.R, p.X, Curve.Order);
        }

        public string GetCompressedHex() {
            return Point.GetCompressedHex();
        }

    }
}
