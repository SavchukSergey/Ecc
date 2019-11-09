using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ecc {
    public readonly struct ECPoint {

        public readonly BigInteger X;

        public readonly BigInteger Y;

        public readonly ECCurve Curve;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        public static ECPoint operator +(in ECPoint left, in ECPoint right) {
            if (left.IsInfinity) return right;
            if (right.IsInfinity) return left;

            var curve = left.Curve;
            var modulus = curve.Modulus;

            var dy = right.Y - left.Y;
            var dx = right.X - left.X;

            if (dx.IsZero && !dy.IsZero) {
                return Infinity;
            }

            var m = dx.IsZero ?
                ((3 * left.X * left.X + curve.A) * ((2 * left.Y).ModInverse(modulus))).ModAbs(modulus) :
                (dy * dx.ModInverse(modulus)).ModAbs(modulus);

            var rx = (m * m - left.X - right.X);
            var ry = (m * (left.X - rx) - left.Y);

            return new ECPoint(rx.ModAbs(modulus), ry.ModAbs(modulus), curve);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in ECPoint a, in ECPoint b) {
            return a.X == b.X && a.Y == b.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in ECPoint a, in ECPoint b) {
            return a.X != b.X || a.Y != b.Y;
        }

        public override int GetHashCode() {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Curve.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj is ECPoint other) {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public static ECPoint operator *(in ECPoint p, BigInteger k) {
            var acc = Infinity;
            var add = p;
            while (k != 0) {
                if (!k.IsEven) acc += add;
                add += add;
                k >>= 1;
            }
            return acc;
        }

        public string GetHex(bool compress = true) {
            if (IsInfinity) return "00";
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

        public bool IsInfinity {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return X.IsZero && Y.IsZero;
            }
        }

        public override string ToString() => $"{{X: {X.ToHexUnsigned(Curve.KeySize8)}, Y: {Y.ToHexUnsigned(Curve.KeySize8)}}}";

        private static readonly ECPoint _infinity = new ECPoint(0, 0, null);

        public static ref readonly ECPoint Infinity => ref _infinity;

    }
}
