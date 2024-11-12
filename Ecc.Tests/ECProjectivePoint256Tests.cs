using Ecc.Math;
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
