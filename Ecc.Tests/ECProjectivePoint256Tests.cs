using NUnit.Framework;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECProjectivePoint256))]
    public class ECProjectivePoint256Tests {

        [Test]
        public void AddTest() {
            var curve = ECCurve256.Secp256k1;
            var affinePointA = curve.CreateKeyPair().PublicKey.Point;
            var affinePointB = curve.CreateKeyPair().PublicKey.Point;

            var affineSumResult = affinePointA + affinePointB;

            var projectivePointA = new ECProjectivePoint256(affinePointA);
            var projectivePointB = new ECProjectivePoint256(affinePointB);

            var projectiveSumResult = (projectivePointA + projectivePointB).ToAffinePoint();

            AssertExt.AssertEquals(affineSumResult, projectiveSumResult);
        }

        [Test]
        public void DoubleTest() {
            var curve = ECCurve256.Secp256k1;
            var affinePointA = curve.CreateKeyPair().PublicKey.Point;

            var affineSumResult = affinePointA.Double();

            var projectivePointA = new ECProjectivePoint256(affinePointA);

            var projectiveSumResult = projectivePointA.Double().ToAffinePoint();

            AssertExt.AssertEquals(affineSumResult, projectiveSumResult);
        }
    }
}
