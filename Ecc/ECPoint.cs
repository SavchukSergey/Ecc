using System.Numerics;

namespace Ecc {
    public class ECPoint {

        public BigInteger X;

        public BigInteger Y;

        public ECCurve Curve;

        public bool Valid {
            get {
                return Curve.Has(this);
            }
        }

        public static ECPoint operator +(ECPoint p, ECPoint q) {
            if (p == null) return q;
            if (q == null) return p;

            var curve = p.Curve;
            var modulus = curve.Modulus;

            var dy = p.Y - q.Y;
            var dx = p.X - q.X;

            if (dx != 0) {
                var invDx = dx.ModInverse(modulus);
                var check = (dx * invDx).ModAbs(modulus);
                var m = dy * invDx;

                var rx = (m * m - p.X - q.X) % modulus;
                var ry = (m * (p.X - rx) - p.Y) % modulus;

                return new ECPoint {
                    X = rx.ModAbs(modulus),
                    Y = ry.ModAbs(modulus),
                    Curve = curve
                };
            } else {
                if (p.Y == 0 && q.Y == 0) {
                    return null;
                } else if (dy == 0) {
                    var m = (3 * p.X * p.X + curve.A) * ((2 * p.Y).ModInverse(modulus));
                    var rx = m * m - 2 * p.X;
                    var ry = (m * (p.X - rx) - p.Y) % modulus;
                    return new ECPoint {
                        X = rx.ModAbs(modulus),
                        Y = ry.ModAbs(modulus),
                        Curve = curve
                    };
                } else {
                    return null;
                }
            }

        }

        public static ECPoint operator *(ECPoint p, BigInteger k) {
            ECPoint acc = null;
            ECPoint add = p;
            while (k != 0) {
                if (!k.IsEven) acc += add;
                add = add + add;
                k = k >> 1;
            }
            return acc;
        }

        public override string ToString() => $"{{X: {X.ToHexUnsigned()}, Y: {Y.ToHexUnsigned()}}}";

    }
}
