using System.Numerics;
using Ecc.Math;
using NUnit.Framework;

namespace Ecc.Tests {
    public static class AssertExt {

        public static void AssertEquals(string expected, in BigInteger512 actual, string? message = null) {
            Assert.That(actual.ToHexUnsigned().ToLowerInvariant(), Is.EqualTo(expected.ToLowerInvariant()), message);
        }

        public static void AssertEquals(string expected, in BigInteger256 actual, string? message = null) {
            Assert.That(actual.ToHexUnsigned().ToLowerInvariant(), Is.EqualTo(expected.ToLowerInvariant()), message);
        }

        public static void AssertEquals(in BigInteger256 expected, in BigInteger256 actual, string? message = null) {
            Assert.That(actual.ToHexUnsigned(), Is.EqualTo(expected.ToHexUnsigned()), message);
        }

        public static void AssertEquals(in BigInteger expected, in BigInteger256 actual, string? message = null) {
            AssertEquals(new BigInteger256(expected), actual, message);
        }

        public static void AssertEquals(in ECPoint256 expected, in ECPoint256 actual, string? path = null) {
            Assert.That(actual.X.ToHexUnsigned(), Is.EqualTo(expected.X.ToHexUnsigned()), $"{path}.X");
            Assert.That(actual.Y.ToHexUnsigned(), Is.EqualTo(expected.Y.ToHexUnsigned()), $"{path}.Y");
        }

        public static void AssertEquals(in ECProjectivePoint256 expected, in ECProjectivePoint256 actual, string? path = null) {
            Assert.That(actual.X.ToHexUnsigned(), Is.EqualTo(expected.X.ToHexUnsigned()), $"{path}.X");
            Assert.That(actual.Y.ToHexUnsigned(), Is.EqualTo(expected.Y.ToHexUnsigned()), $"{path}.Y");
            Assert.That(actual.Z.ToHexUnsigned(), Is.EqualTo(expected.Z.ToHexUnsigned()), $"{path}.Z");
        }

        public static void AssertEquals(in ECProjectiveMontgomeryPoint256 expected, in ECProjectiveMontgomeryPoint256 actual, string? path = null) {
            Assert.That(actual.X.ToHexUnsigned(), Is.EqualTo(expected.X.ToHexUnsigned()), $"{path}.X");
            Assert.That(actual.Y.ToHexUnsigned(), Is.EqualTo(expected.Y.ToHexUnsigned()), $"{path}.Y");
            Assert.That(actual.Z.ToHexUnsigned(), Is.EqualTo(expected.Z.ToHexUnsigned()), $"{path}.Z");
        }

    }
}
