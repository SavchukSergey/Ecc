using Ecc.EC256;
using Ecc.Math;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECPoint256))]
    public class ECPoint256Tests {

        [Test]
        public void AddTest() {
            var curve = new ECCurve256(name: "", a: new BigInteger256(126), b: new BigInteger256(3), modulus: new BigInteger256(127), order: default, cofactor: default, gx: default, gy: default);
            var p = curve.CreatePoint(new BigInteger256(16), new BigInteger256(20));
            var q = curve.CreatePoint(new BigInteger256(41), new BigInteger256(120));
            var r = p + q;
            ClassicAssert.AreEqual(new BigInteger256(86), r.X);
            ClassicAssert.AreEqual(new BigInteger256(81), r.Y);
        }

        [TestCase(16, 20, 97, 81)]
        [TestCase(41, 120, 42, 95)]
        public void AddSameTest(int sx, int sy, int tx, int ty) {
            var curve = new ECCurve256(name: "", a: new BigInteger256(126), b: new BigInteger256(3), modulus: new BigInteger256(127), order: default, cofactor: default, gx: default, gy: default);
            var p = curve.CreatePoint(new BigInteger256((uint)sx), new BigInteger256((uint)sy));
            var r = p + p;
            ClassicAssert.AreEqual(new BigInteger256((uint)tx), r.X);
            ClassicAssert.AreEqual(new BigInteger256((uint)ty), r.Y);
        }

        [TestCase(1, 3, 6)]
        [TestCase(2, 80, 10)]
        [TestCase(3, 80, 87)]
        [TestCase(4, 3, 91)]
        [TestCase(5, int.MaxValue, int.MaxValue)]
        [TestCase(6, 3, 6)]
        public void MulShortTest(int mul, int x, int y) {
            var curve = new ECCurve256(name: "", a: new BigInteger256(2), b: new BigInteger256(3), modulus: new BigInteger256(97), order: default, cofactor: default, gx: default, gy: default);
            var p1 = curve.CreatePoint(new BigInteger256(3), new BigInteger256(6));
            var p2 = p1 * new BigInteger256((uint)mul);
            if (p2.IsInfinity) {
                ClassicAssert.IsTrue(x == int.MaxValue && y == int.MaxValue);
            } else {
                ClassicAssert.AreEqual(new BigInteger256((uint)x), p2.X);
                ClassicAssert.AreEqual(new BigInteger256((uint)y), p2.Y);
            }
        }

        [Test]
        public void GetHexTest() {
            ClassicAssert.AreEqual("0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798", ECCurve256.Secp256k1.G.GetHex());
            ClassicAssert.AreEqual("0479be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8", ECCurve.Secp256k1.G.GetHex(false));
        }

        [TestCase(
            "b47e53eb152729cdb4405719e1097aa43b8b208f55da470fcdc371d438f32c0d",
            "25fb257d4787d72cc560759ef7c97c0e7c97e9c503aa50de3623d9ab3d509f82",
            "02eda473c95637483eb6a5ed9b105423b57fa1ba8b445e9d38068fafaa8bad4c81",
            Secp256k1Curve.CURVE_NAME
        )]
        [TestCase(
            "b47e53eb152729cdb4405719e1097aa43b8b208f55da470fcdc371d438f32c0d",
            "25fb257d4787d72cc560759ef7c97c0e7c97e9c503aa50de3623d9ab3d509f82",
            "03b359fbe006c3016490e0bd17dea2fc13a4ac7a5919cd93e4f90f4b1481cb2d6c",
            NistP256Curve.CURVE_NAME
        )]
        public void AddPointsTest(string k1hex, string k2hex, string pubHex, string curveName) {
            var d1 = BigInteger256.ParseHexUnsigned(k1hex);
            var d2 = BigInteger256.ParseHexUnsigned(k2hex);
            var curve = ECCurve256.GetNamedCurve(curveName);
            var k1 = curve.CreatePrivateKey(d1);
            var k2 = curve.CreatePrivateKey(d2);
            var k3 = curve.CreatePrivateKey(d1 + d2);
            var pub1 = k1.PublicKey;
            var pub2 = k2.PublicKey;
            var pubSum = pub1.Point + pub2.Point;
            var pubSum2 = k3.PublicKey.Point;
            ClassicAssert.AreEqual(pubHex, pubSum.GetHex());
            ClassicAssert.AreEqual(pubHex, pubSum2.GetHex());
        }

        [Test]
        public void NegateTest() {
            var neg = ECCurve256.Secp256k1.G.Negate();
            var res = ECCurve256.Secp256k1.G + neg;
            Assert.That(res.IsInfinity, Is.True);
        }

        [Test]
        public void InfinityGetHexTest() {
            ClassicAssert.AreEqual("00", ECCurve256.Secp256k1.Infinity.GetHex());
        }

    }
}
