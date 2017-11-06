using System.Numerics;
using System.Text;

namespace Ecc {
    public class ECPoint {

        public readonly BigInteger X;

        public readonly BigInteger Y;

        public readonly ECCurve Curve;

        public ECPoint(BigInteger x, BigInteger y, ECCurve curve) {
            X = x;
            Y = y;
            Curve = curve;
        }

        public bool Valid {
            get {
                return Curve.Has(this);
            }
        }

        public static ECPoint operator +(ECPoint p, ECPoint q) {
            if (p == Infinity) return q;
            if (q == Infinity) return p;

            var curve = p.Curve;
            var modulus = curve.Modulus;

            var dy = p.Y - q.Y;
            var dx = p.X - q.X;

            if (dx != 0) {
                var invDx = dx.ModInverse(modulus);
                var check = (dx * invDx).ModAbs(modulus);
                var m = dy * invDx;

                var rx = (m * m - p.X - q.X);
                var ry = (m * (p.X - rx) - p.Y);

                return new ECPoint(rx.ModAbs(modulus), ry.ModAbs(modulus), curve);
            } else {
                if (p.Y == 0 && q.Y == 0) {
                    return Infinity;
                } else if (dy == 0) {
                    var m = (3 * p.X * p.X + curve.A) * ((2 * p.Y).ModInverse(modulus));
                    var rx = m * m - 2 * p.X;
                    var ry = (m * (p.X - rx) - p.Y);
                    return new ECPoint(rx.ModAbs(modulus), ry.ModAbs(modulus), curve);
                } else {
                    return Infinity;
                }
            }

        }

        public static ECPoint operator *(ECPoint p, BigInteger k) {
            var acc = Infinity;
            var add = p;
            while (k != 0) {
                if (!k.IsEven) acc += add;
                add = add + add;
                k = k >> 1;
            }
            return acc;
        }

        public string GetHex(bool compress = true) {
            if (this == Infinity) return "00";
            var keySize = Curve.KeySize8;
            if (compress) {
                var sb = new StringBuilder();
                if (Y.IsEven) sb.Append("02");
                else sb.Append("03");
                sb.Append(X.ToHexUnsigned(keySize));
                return sb.ToString();
            }
            return $"04{X.ToHexUnsigned(keySize)}{Y.ToHexUnsigned(keySize)}";
        }

        public override string ToString() => $"{{X: {X.ToHexUnsigned(Curve.KeySize8)}, Y: {Y.ToHexUnsigned(Curve.KeySize8)}}}";

        public static readonly ECPoint Infinity = new ECPoint(0, 0, null);

    }
}
