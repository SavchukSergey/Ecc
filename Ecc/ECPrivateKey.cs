using System.Numerics;

namespace Ecc {
    public class ECPrivateKey {

        public ECCurve Curve;

        public BigInteger D;

        public ECPublicKey PublicKey => new ECPublicKey { Curve = Curve, Point = Curve.G * D };

        public ECSignature Sign(byte[] hash) {
            var num = Curve.TruncateHash(hash);
            return Sign(num);
        }

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

        public static ECPrivateKey ParseHex(string hex, ECCurve curve) {
            return new ECPrivateKey {
                Curve = curve,
                D = ParseInt(hex)
            };
        }

        private static BigInteger RandomInt(ECCurve curve) {
            var cng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var privData = new byte[curve.Order.ToByteArray().Length];
            cng.GetBytes(privData);
            return new BigInteger(privData).ModAbs(curve.Order);
        }

        private static BigInteger ParseInt(string val) {
            if (val.StartsWith("0x")) val = "0" + val.Substring(2);
            else val = "00" + val;
            return BigInteger.Parse(val, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

    }
}
