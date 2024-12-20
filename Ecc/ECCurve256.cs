﻿using System;
using System.Collections.Generic;
using System.Numerics;
using Ecc.EC256;
using Ecc.Math;

namespace Ecc {
    public class ECCurve256 {

        public readonly string Name;

        public readonly BigInteger256 A;
        public readonly BigInteger256 B;
        public readonly BigInteger256 Modulus;
        public readonly BigInteger256 Order;
        public readonly BigInteger256 Cofactor;
        public readonly ECPoint256 G;

        public readonly int KeySize;

        public readonly int KeySize8;

        public readonly int OrderSize;

        public readonly MontgomeryContext256 MontgomeryContext;

        public ECCurve256(string name,
                   in BigInteger256 a, in BigInteger256 b,
                   in BigInteger256 modulus, in BigInteger256 order,
                   in BigInteger256 cofactor, in BigInteger256 gx, in BigInteger256 gy) {
            Name = name;
            A = a;
            B = b;
            Modulus = modulus;
            Order = order;
            Cofactor = cofactor;
            G = CreatePoint(gx, gy);
            OrderSize = Order.Log2();
            KeySize = (int)Modulus.Log2();
            KeySize8 = (KeySize + 7) >> 3;
            MontgomeryContext = new MontgomeryContext256(Modulus); // todo: precalc it
        }

        public bool Singular {
            get {
                return A.ModCube(Modulus).ModMul(new BigInteger256(4), Modulus).ModAdd(
                    B.ModSquare(Modulus).ModMul(new BigInteger256(27), Modulus),
                    Modulus
                ).IsZero;
            }
        }

        public bool Has(in ECPoint256 p) {
            if (p.IsInfinity) return true;

            var left = p.Y.ModSquare(Modulus);
            var right = p.X.ModCube(Modulus).ModAdd(A.ModMul(p.X, Modulus), Modulus).ModAdd(B, Modulus);
            return left == right;
        }

        public virtual void MulG(in BigInteger256 k, out ECPoint256 result) {
            result = G * k;
        }

        public ECPublicKey256 GetPublicKey(in BigInteger256 k) {
            MulG(in k, out var point);
            return new ECPublicKey256(point);
        }

        public ECPoint256 CreatePoint(in BigInteger256 x, in BigInteger256 y) {
            return new ECPoint256(x, y, this);
        }

        public ECPoint256 CreatePoint(in BigInteger256 x, bool yOdd) {
            var right = x.ModCube(Modulus).ModAdd(
                A.ModMul(x, Modulus).ModAdd(B, Modulus),
                Modulus
            );
            var y = right.ModSqrt(Modulus);
            return CreatePoint(x, y);
        }

        public ECPoint256 CreatePoint(string hex) {
            if (hex == "00") {
                return Infinity;
            }
            if (hex.StartsWith("02")) {
                var x = BigInteger256.ParseHexUnsigned(hex.AsSpan(2));
                return CreatePoint(x, false);
            } else if (hex.StartsWith("03")) {
                var x = BigInteger256.ParseHexUnsigned(hex.AsSpan(2));
                return CreatePoint(x, true);
            } else if (hex.StartsWith("04")) {
                var keySize = KeySize8 * 2;
                var x = BigInteger256.ParseHexUnsigned(hex.AsSpan(2, keySize));
                var y = BigInteger256.ParseHexUnsigned(hex.AsSpan(keySize + 2));
                return CreatePoint(x, y);
            }
            throw new System.FormatException();
        }

        public ECPrivateKey256 CreateKeyPair() {
            return ECPrivateKey256.Create(this);
        }

        public ECPrivateKey256 CreatePrivateKey(string hex) {
            return ECPrivateKey256.ParseHex(hex, this);
        }

        public ECPrivateKey256 CreatePrivateKey(ReadOnlySpan<byte> data) {
            return new ECPrivateKey256(BigInteger256Ext.FromBigEndianBytes(data), this);
        }

        public ECPrivateKey256 CreatePrivateKey(in BigInteger256 data) {
            return new ECPrivateKey256(data, this);
        }

        public ECPublicKey256 CreatePublicKey(string hex) {
            var point = CreatePoint(hex);
            return new ECPublicKey256(point);
        }

        public BigInteger256 TruncateHash(ReadOnlySpan<byte> hash) {
            return new BigInteger256(hash[..(OrderSize >> 3)], bigEndian: true);
        }

        public static readonly ECCurve256 Secp256k1 = new Secp256k1Curve();

        public static readonly ECCurve256 NistP256 = new NistP256Curve();

        public static ECCurve256 GetNamedCurve(string name) {
            return name switch {
                Secp256k1Curve.CURVE_NAME => Secp256k1,
                NistP256Curve.CURVE_NAME => NistP256,
                _ => throw new Exception($"unknown curve: {name}"),
            };
        }

        public static IEnumerable<ECCurve256> GetNamedCurves() {
            yield return Secp256k1;
            yield return NistP256;
        }

        public override int GetHashCode() {
            if (!string.IsNullOrWhiteSpace(Name)) {
                return Name.GetHashCode();
            }
            return Modulus.GetHashCode() ^ A.GetHashCode() ^ B.GetHashCode() ^ G.X.GetHashCode() ^ G.Y.GetHashCode() ^ Order.GetHashCode() ^ Cofactor.GetHashCode();
        }

        public ECPoint256 Infinity => new(new BigInteger256(0), new BigInteger256(0), this);

    }
}
