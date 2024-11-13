using System.Runtime.CompilerServices;
using Ecc.Math;

//https://www.nayuki.io/page/elliptic-curve-point-addition-in-projective-coordinates

namespace Ecc {
    public readonly struct ECProjectiveMontgomeryPoint256 {

        public readonly BigInteger256 X;

        public readonly BigInteger256 Y;

        public readonly BigInteger256 Z;

        public readonly ECCurve256 Curve;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ECProjectiveMontgomeryPoint256(in BigInteger256 x, in BigInteger256 y, ECCurve256 curve) {
            X = x;
            Y = y;
            Z = curve.MontgomeryContext.One;
            Curve = curve;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ECProjectiveMontgomeryPoint256(in BigInteger256 x, in BigInteger256 y, in BigInteger256 z, ECCurve256 curve) {
            X = x;
            Y = y;
            Z = z;
            Curve = curve;
        }

        public readonly ECProjectiveMontgomeryPoint256 Double() {
            ref readonly MontgomeryContext256 ctx = ref Curve.MontgomeryContext;

            var x2mont = ctx.ModSquare(X);
            var z2mont = ctx.ModSquare(Z);
            var amont = ctx.ToMontgomery(Curve.A); //todo: cache
            var tmont = ctx.ModAdd(
                ctx.ModTripple(x2mont),
                ctx.ModMul(amont, z2mont)
            );

            var umont = ctx.ModDouble(ctx.ModMul(Y, Z));
            var uymont = ctx.ModMul(Y, umont);
            var vmont = ctx.ModDouble(ctx.ModMul(X, uymont));
            var t2mont = ctx.ModSquare(tmont);
            var wmont = ctx.ModSub(t2mont,
                ctx.ModDouble(vmont)
            );

            var resX = ctx.ModMul(umont, wmont);
            var resY = ctx.ModSub(
                ctx.ModMul(
                    tmont,
                    ctx.ModSub(vmont, wmont)
                ),
                ctx.ModDouble(
                    ctx.ModSquare(uymont)
                )
            );
            var resZ = ctx.ModCube(umont);
            return new ECProjectiveMontgomeryPoint256(
                resX,
                resY,
                resZ,
                Curve
            );
        }

        public static ECProjectiveMontgomeryPoint256 operator +(in ECProjectiveMontgomeryPoint256 left, in ECProjectiveMontgomeryPoint256 right) {
            if (left.IsInfinity) return right;
            if (right.IsInfinity) return left;

            ref readonly MontgomeryContext256 ctx = ref left.Curve.MontgomeryContext;

            ctx.ModMul(left.X, right.Z, out var lxz);
            ctx.ModMul(right.X, left.Z, out var rxz);

            ctx.ModMul(left.Y, right.Z, out var lyz);
            ctx.ModMul(right.Y, left.Z, out var ryz);
            if (lxz == rxz) {

                if (lyz == ryz) {
                    return left.Double();
                }
                return new ECProjectiveMontgomeryPoint256(
                    new BigInteger256(),
                    new BigInteger256(),
                    ctx.One,
                    left.Curve
                );
            }

            ctx.ModSub(lyz, ryz, out var t);

            ctx.ModSub(lxz, rxz, out var u);

            ctx.ModSquare(u, out var u2);
            ctx.ModMul(u2, u, out var u3);

            ctx.ModMul(left.Z, right.Z, out var v);

            ctx.ModSquare(t, out var t2);

            var w = ctx.ModSub(
                ctx.ModMul(t2, v),
                ctx.ModMul(
                    u2,
                    ctx.ModAdd(
                        lxz,
                        rxz
                    )
                )
            );

            ctx.ModMul(u, w, out var resX);
            var resY = ctx.ModSub(
                ctx.ModMul(
                    t,
                    ctx.ModSub(
                        ctx.ModMul(lxz, u2),
                        w
                    )
                ),
                ctx.ModMul(lyz, u3)
            );
            ctx.ModMul(u3, v, out var resZ);

            return new ECProjectiveMontgomeryPoint256(
                resX,
                resY,
                resZ,
                left.Curve
            );
        }

        public ECProjectivePoint256 Reduce() {
            return new ECProjectivePoint256(
                Curve.MontgomeryContext.Reduce(X),
                Curve.MontgomeryContext.Reduce(Y),
                Curve.MontgomeryContext.Reduce(Z),
                Curve
            );
        }

        public ECProjectiveMontgomeryPoint256 Normalize() {
            var reduced = Reduce();
            var norm = reduced.Normalize();
            return new ECProjectiveMontgomeryPoint256(
                norm.X,
                norm.Y,
                norm.Z,
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
