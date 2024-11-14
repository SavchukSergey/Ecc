using Ecc.EC256;
using Ecc.Math;
using NUnit.Framework;

namespace Ecc.Tests.EC256 {
    [TestFixture]
    [TestOf(typeof(Secp256k1Curve))]
    public class Secp256k1CurveTests {

        [Test]
        public void PublicKeyTest() {
            var multiplier = BigInteger256.ParseHexUnsigned("b359fbe006c3016490e0bd17dea2fc13a4ac7a5919cd93e4f90f4b1481cb2d6c");

            var curve = ECCurve256.Secp256k1;
            var result = curve.CreatePrivateKey(multiplier).PublicKey.Point;
            AssertExt.AssertEquals(new ECPoint256(
                BigInteger256.ParseHexUnsigned("d5859b6fda02647f786e1f0e8c62ab657dec000b0e6f82c7f232e4786b4e1744"),
                BigInteger256.ParseHexUnsigned("a8737ef78f00432c99bd0948394a9b75fe14a9d3a0d9361e92208a9023f442c2"),
                curve
            ), result);
        }

    }
}
