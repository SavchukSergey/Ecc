using System;
using System.Data;
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

        public readonly ECPoint256 Double() {
            return this + this;
        }

        public readonly ECPoint256 ShiftLeft(int count) {
            var walker = this;
            while (count > 0) {
                walker = walker.Double();
                count--;
            }
            return walker;
        }

        public static ECPoint256 operator +(in ECPoint256 left, in ECPoint256 right) {
            if (left.IsInfinity) return right;
            if (right.IsInfinity) return left;

            var curve = left.Curve;

            var dy = right.Y.ModSub(left.Y, curve.Modulus);
            var dx = right.X.ModSub(left.X, curve.Modulus);

            if (dx.IsZero && !dy.IsZero) {
                return new ECPoint256(new BigInteger256(), new BigInteger256(), curve);
            }

            //todo: left.X.ModSquare(curve.Modulus) can be cached in point
            var m = dx.IsZero ?
                left.X.ModSquare(curve.Modulus).ModTriple(curve.Modulus).ModAdd(curve.A, curve.Modulus).ModDiv(left.Y.ModDouble(curve.Modulus), curve.Modulus) :
                dy.ModDiv(dx, curve.Modulus);

            // var mmont = curve.MontgomeryContext.ToMontgomery(m);
            m.ModSquare(curve.Modulus, out var m2);
            var rx = m2.ModSub(left.X, curve.Modulus).ModSub(right.X, curve.Modulus);
            var ry = m.ModMul(left.X.ModSub(rx, curve.Modulus), curve.Modulus).ModSub(left.Y, curve.Modulus);

            return new ECPoint256(
                rx,
                ry,
                curve
            );
        }

        public ECProjectivePoint256 ToProjective() {
            return new ECProjectivePoint256(this);
        }

        public static ECPoint256 operator -(in ECPoint256 left, in ECPoint256 right) {
            return left + right.Negate();
        }

        public readonly ECPoint256 Negate() {
            if (IsInfinity) {
                return this;
            }
            return new ECPoint256(X, Curve.Modulus - Y, Curve);
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
            var result = p.Curve.Infinity;
            var acc = p;
            var zeroes = 0;
            var ones = 0;
            for (var i = 0; i < BigInteger256.BITS_SIZE + 1; i++) {
                if (i < BigInteger256.BITS_SIZE && k.GetBit(i)) {
                    ones++;
                } else {
                    zeroes++;
                    if (ones > 0) {
                        acc = acc.ShiftLeft(zeroes - 1);

                        // we start getting profit only if ones >= 3

                        // if have two ones i.e.pattern 011
                        // using simple bit approach: we need one double, two adds
                        // using RLE bit approach: we need two doubles and one sub

                        // if have three ones i.e.pattern 0111
                        // using simple bit approach: we need two double, three adds
                        // using RLE bit approach: we need four doubles and one sub

                        if (ones >= 3) {
                            result -= acc;
                            acc = acc.ShiftLeft(ones);
                            ones = 0;
                            result += acc;
                            zeroes = 1;
                        } else {
                            while (ones > 0) {
                                result += acc;
                                acc = acc.Double();
                                ones--;
                            }
                            zeroes = 1;
                        }

                    }
                }
            }
            return result;
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

    }
}
