﻿using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ecc {
    public readonly struct ECPoint {

        public readonly BigInteger X;

        public readonly BigInteger Y;

        public readonly ECCurve Curve;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ECPoint(in BigInteger x, in BigInteger y, ECCurve curve) {
            X = x;
            Y = y;
            Curve = curve;
        }

        public readonly bool Valid {
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
                ((3 * left.X * left.X + curve.A) * (2 * left.Y).ModInverse(modulus)).ModAbs(modulus) :
                (dy * dx.ModInverse(modulus)).ModAbs(modulus);

            var rx = m * m - left.X - right.X;
            var ry = m * (left.X - rx) - left.Y;

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

        public override bool Equals(object? obj) {
            if (obj is ECPoint other) {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public static ECPoint operator *(in ECPoint p, in BigInteger k) {
            var acc = Infinity;
            var add = p;
            Span<byte> data = stackalloc byte[k.GetByteCount(isUnsigned: true)];
            k.TryWriteBytes(data, out var _, isUnsigned: true);
            for (var i = 0; i < data.Length; i++) {
                var bt = data[i];
                for (var bit = 1; bit < 256; bit <<= 1) {
                    if ((bt & bit) != 0) {
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
                X.ToBigEndianBytes(output[1..], keySize);
            } else {
                output[0] = 4;
                X.ToBigEndianBytes(output[1..], keySize);
                Y.ToBigEndianBytes(output[(1 + keySize)..], keySize);
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

        public override string ToString() => $"{{X: {X.ToHexUnsigned(Curve.KeySize8)}, Y: {Y.ToHexUnsigned(Curve.KeySize8)}}}";

        private static readonly ECPoint _infinity = new ECPoint(0, 0, null!);

        public static ref readonly ECPoint Infinity => ref _infinity;

    }
}
