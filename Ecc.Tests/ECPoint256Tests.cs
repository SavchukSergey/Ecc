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

        [TestCase(
            "262331267d5dc527a9b4225337671f75228e97ea7137ea78a3fa7314bd1cedae",
            "cd35a5325fb802543006d5891056c9bde2b3683a58c945ec80d74b180baf78ee",
            "e456fcc29e11d05faf31c251089b9a6c09ab1d82aec8672bba416e1cd45d055e",
            "f5c02e7c505d6051c94f3ab38bf8a2b8039ec5931e7f7cb3924d7dac1be353e6"
        )]
        [TestCase(
            "e456fcc29e11d05faf31c251089b9a6c09ab1d82aec8672bba416e1cd45d055e",
            "f5c02e7c505d6051c94f3ab38bf8a2b8039ec5931e7f7cb3924d7dac1be353e6",
            "33513202c960d2cbb194e4ab5c42dce572153cda471a1de67c48e14b82817e21",
            "40eadf05d67d14771058661230346b5c3ec2f11d3e8eb67a55a9b4317e5b72fd"
        )]
        public void DoubleTest(string sourceX, string sourceY, string expectedX, string expectedY) {
            var point = new ECPoint256(
                  BigInteger256.ParseHexUnsigned(sourceX),
                  BigInteger256.ParseHexUnsigned(sourceY),
                  ECCurve256.Secp256k1
            );
            var res = point.Double();
            AssertExt.AssertEquals(new ECPoint256(
                  BigInteger256.ParseHexUnsigned(expectedX),
                  BigInteger256.ParseHexUnsigned(expectedY),
                  ECCurve256.Secp256k1
            ), res);
        }


        [Test]
        public void MulKTest() {
            var point = new ECPoint256(
                  BigInteger256.ParseHexUnsigned("262331267d5dc527a9b4225337671f75228e97ea7137ea78a3fa7314bd1cedae"),
                  BigInteger256.ParseHexUnsigned("cd35a5325fb802543006d5891056c9bde2b3683a58c945ec80d74b180baf78ee"),
                  ECCurve256.Secp256k1
            );
            var k = BigInteger256.ParseHexUnsigned("b359fbe006c3016490e0bd17dea2fc13a4ac7a5919cd93e4f90f4b1481cb2d6c");
            var res = point * k;
            AssertExt.AssertEquals(new ECPoint256(
                  BigInteger256.ParseHexUnsigned("1eb65a6bf5fc5f8e5adbe85b111d5c232abdace60a0b3f57f408c287a867c347"),
                  BigInteger256.ParseHexUnsigned("fd0dc20576138c748934c1185faa24740c72dd688a0d7d7ed2bead2f10f72411"),
                  ECCurve256.Secp256k1
            ), res);
        }

        [Test]
        public void InfinityGetHexTest() {
            ClassicAssert.AreEqual("00", ECCurve256.Secp256k1.Infinity.GetHex());
        }

    }
}
