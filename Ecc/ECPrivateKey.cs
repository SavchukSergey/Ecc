using System.Numerics;

namespace Ecc {
    public class ECPrivateKey {

        public ECCurve Curve;

        public BigInteger D;

        public ECPublicKey PublicKey => new ECPublicKey { Curve = Curve, Point = Curve.G * D };

        public ECSignature Sign(BigInteger message) {
            BigInteger r = new BigInteger(0);
            BigInteger s = new BigInteger(0);
            while (r == 0 || s == 0) {
                var k = RandomInt(Curve);
                var p = Curve.G * k;
                r = p.X % Curve.Order;
                s = ((message + r * D) * BigIntegerExt.ModInverse(k, Curve.Order)) % Curve.Order;
            }
            return new ECSignature {
                R = r,
                S = s
            };
        }

        public static ECPrivateKey Create(ECCurve curve) {
            var priv = RandomInt(curve);
            //todo: check not zero
            return new ECPrivateKey {
                D = priv,
                Curve = curve
            };
        }

        private static BigInteger RandomInt(ECCurve curve) {
            var cng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var privData = new byte[curve.Order.ToByteArray().Length];
            cng.GetBytes(privData);
            return new BigInteger(privData).ModAbs(curve.Order);
        }

    }
}
