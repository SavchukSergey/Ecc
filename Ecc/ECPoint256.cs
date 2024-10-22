using System;
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

            var dy = right.Y.ModSub(left.Y, curve.Modulus);
            var dx = right.X.ModSub(left.X, curve.Modulus);

            if (dx.IsZero && !dy.IsZero) {
                return Infinity;
            }

            //todo: left.X.ModSquare(curve.Modulus) can be cached in point
            var m = dx.IsZero ?
                left.X.ModSquare(curve.Modulus).ModTriple(curve.Modulus).ModAdd(curve.A, curve.Modulus).ModDiv(left.Y.ModDouble(curve.Modulus), curve.Modulus) :
                dy.ModDiv(dx, curve.Modulus);

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

        public readonly int GetBytesCount(bool compress = true) {
            if (IsInfinity) {
                return 1;
            }
            var keySize = Curve.KeySize8;
            if (compress) {
                return keySize + 1;
            } else {
                return 2 * keySize + 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetHexSize(bool compress = true) {
            return GetBytesCount(compress) * 2;
        }

        public readonly void GetBytes(Span<byte> output, bool compress = true) {
            if (IsInfinity) {
                output[0] = 0;
                return;
            }
            var keySize = Curve.KeySize8;
            if (compress) {
                output[0] = Y.IsEven ? (byte)2 : (byte)3;
                X.WriteBigEndian(output[1..]);
            } else {
                output[0] = 4;
                X.WriteBigEndian(output[1..]);
                Y.WriteBigEndian(output[(1 + keySize)..]);
            }
        }

        public readonly string GetHex(bool compress = true) {
            Span<char> chars = stackalloc char[GetHexSize(compress)];
            GetHex(chars, compress);
            return new string(chars);
        }

        public readonly void GetHex(Span<char> output, bool compress = true) {
            Span<byte> bytes = stackalloc byte[GetBytesCount(compress)];
            GetBytes(bytes, compress);
            ((ReadOnlySpan<byte>)bytes).ToHexString(output);
        }

        public readonly bool IsInfinity {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return X.IsZero && Y.IsZero;
            }
        }

        public override string ToString() => $"{{X: {X.ToHexUnsigned()}, Y: {Y.ToHexUnsigned()}}}";

        private static readonly ECPoint256 _infinity = new(new BigInteger256(0), new BigInteger256(0), null!);

        public static ref readonly ECPoint256 Infinity => ref _infinity;

    }
}
