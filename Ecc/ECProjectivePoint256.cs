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

            var lxz = left.X.ModMul(left.Z, curve.Modulus);
            var rxz = right.X.ModMul(right.Z, curve.Modulus);

            if (lxz == rxz) {
                var lyz = left.Y.ModMul(left.Z, curve.Modulus);
                var ryz = right.Y.ModMul(right.Z, curve.Modulus);
                if (lyz == ryz) {
                    return left.Double();
                }
                return new ECProjectivePoint256(curve.Infinity);
            }

            var t0 = left.Y.ModMul(right.Z, curve.Modulus);
            var t1 = right.Y.ModMul(left.Z, curve.Modulus);
            var t = t0.ModSub(t1, curve.Modulus);

            var u0 = left.X.ModMul(right.Z, curve.Modulus);
            var u1 = right.X.ModMul(left.Z, curve.Modulus);
            var u = u0.ModSub(u1, curve.Modulus);

            var u2 = u.ModSquare(curve.Modulus);
            var u3 = u2.ModMul(u, curve.Modulus);

            var v = left.Z.ModMul(right.Z, curve.Modulus);
            var t2 = t.ModSquare(curve.Modulus);
            var w = t2.ModMul(v, curve.Modulus).ModSub(
                u2.ModMul(u0.ModAdd(u1, curve.Modulus), curve.Modulus),
                curve.Modulus
            );
            var resX = u.ModMul(w, curve.Modulus);
            var resY = t.ModMul(
                u0.ModMul(u2, curve.Modulus).ModSub(w, curve.Modulus),
                curve.Modulus
            ).ModSub(
                t0.ModMul(u3, curve.Modulus),
                curve.Modulus
            );
            var resZ = u3.ModMul(v, curve.Modulus);

            return new ECProjectivePoint256(
                resX,
                resY,
                resZ,
                curve
            );
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
