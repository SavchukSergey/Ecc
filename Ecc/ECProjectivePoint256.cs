using System.Runtime.CompilerServices;
using Ecc.Math;

//https://www.nayuki.io/page/elliptic-curve-point-addition-in-projective-coordinates

namespace Ecc {
    public readonly struct ECProjectivePoint256 {

        public readonly BigInteger256 X;

        public readonly BigInteger256 Y;

        public readonly BigInteger256 Z;

        public readonly ECCurve256 Curve;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ECProjectivePoint256(in ECPoint256 source) {
            X = source.X;
            Y = source.Y;
            Z = new BigInteger256(1);
            Curve = source.Curve;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ECProjectivePoint256(in BigInteger256 x, in BigInteger256 y, ECCurve256 curve) {
            X = x;
            Y = y;
            Z = new BigInteger256(1);
            Curve = curve;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ECProjectivePoint256(in BigInteger256 x, in BigInteger256 y, in BigInteger256 z, ECCurve256 curve) {
            X = x;
            Y = y;
            Z = z;
            Curve = curve;
        }

        public readonly ECProjectivePoint256 Double() {
            var x2 = X.ModSquare(Curve.Modulus);
            var z2 = Z.ModSquare(Curve.Modulus);
            var t = x2.ModTriple(Curve.Modulus).ModAdd(
                Curve.A.ModMul(z2, Curve.Modulus),
                Curve.Modulus
            );
            var u = Y.ModMul(Z, Curve.Modulus).ModDouble(Curve.Modulus);
            var uy = Y.ModMul(u, Curve.Modulus);
            var v = X.ModMul(uy, Curve.Modulus).ModDouble(Curve.Modulus);
            var t2 = t.ModSquare(Curve.Modulus);
            var w = t2.ModSub(v.ModDouble(Curve.Modulus), Curve.Modulus);

            var resX = u.ModMul(w, Curve.Modulus);
            var resY = t.ModMul(
                v.ModSub(w, Curve.Modulus),
                Curve.Modulus
            ).ModSub(
                uy.ModSquare(Curve.Modulus).ModDouble(Curve.Modulus),
                Curve.Modulus
            );
            var resZ = u.ModCube(Curve.Modulus);
            return new ECProjectivePoint256(
                resX,
                resY,
                resZ,
                Curve
            );
        }

        public static ECProjectivePoint256 operator +(in ECProjectivePoint256 left, in ECProjectivePoint256 right) {
            if (left.IsInfinity) return right;
            if (right.IsInfinity) return left;

            var curve = left.Curve;

            var dy = right.Y.ModSub(left.Y, curve.Modulus);
            var dx = right.X.ModSub(left.X, curve.Modulus);

            if (dx.IsZero && !dy.IsZero) {
                return new ECProjectivePoint256(new BigInteger256(), new BigInteger256(), curve);
            }

            // var m = dx.IsZero ?
            //     left.X.ModSquare(curve.Modulus).ModTriple(curve.Modulus).ModAdd(curve.A, curve.Modulus).ModDiv(left.Y.ModDouble(curve.Modulus), curve.Modulus) :
            //     dy.ModDiv(dx, curve.Modulus);

            // m.ModSquare(curve.Modulus, out var m2);
            // var rx = m2.ModSub(left.X, curve.Modulus).ModSub(right.X, curve.Modulus);
            // var ry = m.ModMul(left.X.ModSub(rx, curve.Modulus), curve.Modulus).ModSub(left.Y, curve.Modulus);

            // return new ECPoint256(
            //     rx,
            //     ry,
            //     curve
            // );



            return new ECProjectivePoint256(left.ToAffinePoint() + right.ToAffinePoint());
        }

        public ECPoint256 ToAffinePoint() {
            var inv = Z.ModInverse(Curve.Modulus);

            return new ECPoint256(
                X.ModMul(inv, Curve.Modulus),
                Y.ModMul(inv, Curve.Modulus),
                Curve
            );
        }

        public readonly bool IsInfinity {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return X.IsZero && Y.IsZero;
            }
        }
    }
}
