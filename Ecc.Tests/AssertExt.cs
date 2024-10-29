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
    }
}
