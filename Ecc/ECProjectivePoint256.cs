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

        public readonly ECProjectivePoint256 Negate() {
            if (IsInfinity) {
                return this;
            }
            return new ECProjectivePoint256(X, Curve.Modulus - Y, Z, Curve);
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

            var lxz = left.X.ModMul(right.Z, curve.Modulus);
            var rxz = right.X.ModMul(left.Z, curve.Modulus);

            var lyz = left.Y.ModMul(right.Z, curve.Modulus);
            var ryz = right.Y.ModMul(left.Z, curve.Modulus);

            var dx = lxz.ModSub(rxz, curve.Modulus);
            var dy = lyz.ModSub(ryz, curve.Modulus);

            if (dx.IsZero) {
                if (dy.IsZero) {
                    return left.Double();
                }
                return new ECProjectivePoint256(curve.Infinity);
            }

            var dx2 = dx.ModSquare(curve.Modulus);
            var dx3 = dx2.ModMul(dx, curve.Modulus);

            var dy2 = dy.ModSquare(curve.Modulus);
            
            var z2 = left.Z.ModMul(right.Z, curve.Modulus);
            
            var w = dy2.ModMul(z2, curve.Modulus).ModSub(
                dx2.ModMul(lxz.ModAdd(rxz, curve.Modulus), curve.Modulus),
                curve.Modulus
            );
            var resX = dx.ModMul(w, curve.Modulus);
            var resY = dy.ModMul(
                lxz.ModMul(dx2, curve.Modulus).ModSub(w, curve.Modulus),
                curve.Modulus
            ).ModSub(
                lyz.ModMul(dx3, curve.Modulus),
                curve.Modulus
            );
            var resZ = dx3.ModMul(z2, curve.Modulus);

            return new ECProjectivePoint256(
                resX,
                resY,
                resZ,
                curve
            );
        }

        public readonly ECProjectiveMontgomeryPoint256 ToMontgomery() {
            return new ECProjectiveMontgomeryPoint256(
                x: Curve.MontgomeryContext.ToMontgomery(X),
                y: Curve.MontgomeryContext.ToMontgomery(Y),
                Curve
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

        public ECProjectivePoint256 Normalize() {
            return new ECProjectivePoint256(ToAffinePoint());
        }

        public readonly bool IsInfinity {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return X.IsZero && Y.IsZero;
            }
        }
    }
}
