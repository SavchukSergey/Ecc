using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Ecc.Math;

namespace Ecc {
    public readonly struct ECPoint256 {

        public readonly BigInteger256 X;

        public readonly BigInteger256 Y;

        public readonly ECCurve256 Curve;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ECPoint256(in BigInteger256 x, in BigInteger256 y, ECCurve256 curve) {
            X = x;
            Y = y;
            Curve = curve;
        }

        public readonly bool Valid {
            get {
                return Curve.Has(this);
            }
        }

        public static ECPoint256 operator +(in ECPoint256 left, in ECPoint256 right) {
            if (left.IsInfinity) return right;
            if (right.IsInfinity) return left;

            var curve = left.Curve;
            var modulus = curve.Modulus;

            var dy = right.Y.ModSub(left.Y, curve.Modulus);
            var dx = right.X.ModSub(left.X, curve.Modulus);

            if (dx.IsZero && !dy.IsZero) {
                return Infinity;
            }

            //todo: left.X.ModSquare(curve.Modulus) can be cached in point
            var m = dx.IsZero ?
                new BigInteger256(3).ModMul(
                    left.X.ModSquare(curve.Modulus),
                    curve.Modulus
                ).ModAdd(curve.A, curve.Modulus).ModDiv(left.Y.ModDouble(curve.Modulus), curve.Modulus) :
                dy.ModDiv(dx, modulus);

            var m2 = m.ModSquare(curve.Modulus);
            var rx = m2.ModSub(left.X, curve.Modulus).ModSub(right.X, curve.Modulus);
            var ry = m.ModMul(left.X.ModSub(rx, curve.Modulus), curve.Modulus).ModSub(left.Y, curve.Modulus);

            return new ECPoint256(
                rx,
                ry,
                curve
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in ECPoint256 a, in ECPoint256 b) {
            return a.X == b.X && a.Y == b.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in ECPoint256 a, in ECPoint256 b) {
            return a.X != b.X || a.Y != b.Y;
        }

        public override int GetHashCode() {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Curve.GetHashCode();
        }

        public override bool Equals(object? obj) {
            if (obj is ECPoint256 other) {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public static ECPoint256 operator *(in ECPoint256 p, in BigInteger256 k) {
            var acc = Infinity;
            var add = p;
            for (var i = 0; i < BigInteger256.BYTES_SIZE; i++) {
                var bt = k.GetByte(i);
                for (var mask = 1; mask < 256; mask <<= 1) {
                    if ((bt & mask) != 0) {
                        acc += add;
                    }
                    add += add;
                }
            }
            return acc;
        }

        public readonly byte[] GetBytes(bool compress = true) {
            if (IsInfinity) return [0];
            var keySize = Curve.KeySize8;
            if (compress) {
                var res = new byte[keySize + 1];
                res[0] = Y.IsEven ? (byte)2 : (byte)3;
                X.ToBigEndianBytes(res, 1, keySize);
                return res;
            } else {
                var res = new byte[2 * keySize + 1];
                res[0] = 4;
                X.ToBigEndianBytes(res, 1, keySize);
                Y.ToBigEndianBytes(res, 1 + keySize, keySize);
                return res;
            }
        }

        public readonly string GetHex(bool compress = true) {
            var bytes = GetBytes(compress);
            return bytes.ToHexString();
        }

        public readonly bool IsInfinity {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return X.IsZero && Y.IsZero;
            }
        }

        public override string ToString() => $"{{X: {X.ToHexUnsigned(Curve.KeySize8)}, Y: {Y.ToHexUnsigned(Curve.KeySize8)}}}";

        private static readonly ECPoint256 _infinity = new ECPoint256(new BigInteger256(0), new BigInteger256(0), null!);

        public static ref readonly ECPoint256 Infinity => ref _infinity;

    }
}
