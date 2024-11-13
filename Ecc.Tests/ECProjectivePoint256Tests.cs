using Ecc.Math;
using NUnit.Framework;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECProjectivePoint256))]
    public class ECProjectivePoint256Tests {

        [TestCase(
            "73c467b2018ef7022c9a162aaf43e4c4b5cb846d4c0c2ec8d1aae52195c8ae53",
            "1cf6ac2793df7f1d72aa6b5a053956a7e61d6466c96af9b829f657dcc5b5cfc6",
            "3204bf067b51771a4b3509ec7de9dfb84622b2cd6564dff69735bcb48377a94e",
            "a3686c2854ff9781d6bea653742575e0dd55c4c0cf327ce9dde25691749da421",
            "33287fe75ee2c097b60cb96c6447c559da0bd6fb3d47f607d1cd466db8ec7823",
            "e57c04d41bbfc43c1572585271b82169feba725ef83b2dd4a2a3dcb08c0ded40"
        )]
        public void AddTest(string leftX, string leftY, string leftZ, string rightX, string rightY, string rightZ) {
            var curve = ECCurve256.Secp256k1;

            var projectivePointA = new ECProjectivePoint256(
                BigInteger256.ParseHexUnsigned(leftX),
                BigInteger256.ParseHexUnsigned(leftY),
                BigInteger256.ParseHexUnsigned(leftZ),
                curve);
            var projectivePointB = new ECProjectivePoint256(
                BigInteger256.ParseHexUnsigned(rightX),
                BigInteger256.ParseHexUnsigned(rightY),
                BigInteger256.ParseHexUnsigned(rightZ),
                curve);
            var affinePointA = projectivePointA.ToAffinePoint();
            var affinePointB = projectivePointB.ToAffinePoint();

            var expectedAffinePoint = affinePointA + affinePointB;

            var projectiveSumPoint = projectivePointA + projectivePointB;
            var actualAffinePoint = projectiveSumPoint.ToAffinePoint();

            AssertExt.AssertEquals(expectedAffinePoint, actualAffinePoint);
        }

        [TestCase(
            "67a90189db61a177ca5ae73135a5c0d4d1c38644db19e5714aa478800d72c47f",
            "93b629c732ffd156073eaea110ec54a7ab2148d20feb4c620dd81b63169b0924",
            "625f314aa043b3e11af031b2f5be8e73ff8af39f0d2eb4b7a38e3ce63a809777"
        )]
        public void DoubleTest(string sourceX, string sourceY, string sourceZ) {
            var curve = ECCurve256.Secp256k1;

            var projectivePointA = new ECProjectivePoint256(
                BigInteger256.ParseHexUnsigned(sourceX),
                BigInteger256.ParseHexUnsigned(sourceY),
                BigInteger256.ParseHexUnsigned(sourceZ),
                curve
            );
            var affinePoint = projectivePointA.ToAffinePoint();
            var expectedAffineResult = affinePoint.Double();

            var projectiveSumResult = projectivePointA.Double();

            var actualAffineSumResult = projectiveSumResult.ToAffinePoint();

            AssertExt.AssertEquals(expectedAffineResult, actualAffineSumResult);
        }

        [Test]
        public void NegateTest() {
            var curve = ECCurve256.Secp256k1;
            var pub1 = curve.CreateKeyPair().PublicKey.Point;
            var pub2 = pub1.Negate();

            var sum = pub1 + pub2;
            Assert.That(sum.IsInfinity, Is.True);
        }

        [Test]
        public void ToAffinePointTest() {
            var curve = ECCurve256.Secp256k1;
            var projectivePoint = new ECProjectivePoint256(
                BigInteger256.ParseHexUnsigned("3ce6ec61299a964dbfd4d5587a14ac30ebb136abb909ea157c3c053acf88582c"),
                BigInteger256.ParseHexUnsigned("c4c3a8137d1426ecd61023b120bfea8968a8eb233d0e63b7ae3759df10383b82"),
                BigInteger256.ParseHexUnsigned("b4e2d4ab187ef003beb7fb6423ef3429d9153bcb78f5743c5ad6564e35842a1f"),
                curve
            );
            var affinePoint = projectivePoint.ToAffinePoint();
            AssertExt.AssertEquals(new ECPoint256(
                BigInteger256.ParseHexUnsigned("262331267d5dc527a9b4225337671f75228e97ea7137ea78a3fa7314bd1cedae"),
                BigInteger256.ParseHexUnsigned("cd35a5325fb802543006d5891056c9bde2b3683a58c945ec80d74b180baf78ee"),
                curve
            ), affinePoint);
        }
    }
}
