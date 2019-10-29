using NUnit.Framework;
using System.Numerics;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECPoint))]
    public class ECPointTests {

        [Test]
        public void AddTest() {
            var curve = new ECCurve(name: null, a: -1, b: 3, modulus: 127, order: default, cofactor: default, gx: default, gy: default);
            var p = curve.CreatePoint(16, 20);
            var q = curve.CreatePoint(41, 120);
            var r = p + q;
            Assert.AreEqual(new BigInteger(86), r.X);
            Assert.AreEqual(new BigInteger(81), r.Y);
        }

        [TestCase(16, 20, 97, 81)]
        [TestCase(41, 120, 42, 95)]
        public void AddSameTest(int sx, int sy, int tx, int ty) {
            var curve = new ECCurve(name: null, a: -1, b: 3, modulus: 127, order: default, cofactor: default, gx: default, gy: default);
            var p = curve.CreatePoint(sx, sy);
            var r = p + p;
            Assert.AreEqual(new BigInteger(tx), r.X);
            Assert.AreEqual(new BigInteger(ty), r.Y);
        }

        [TestCase(1, 3, 6)]
        [TestCase(2, 80, 10)]
        [TestCase(3, 80, 87)]
        [TestCase(4, 3, 91)]
        [TestCase(5, int.MaxValue, int.MaxValue)]
        [TestCase(6, 3, 6)]
        public void MulTest(int mul, int x, int y) {
            var curve = new ECCurve(name: null, a: 2, b: 3, modulus: 97, order: default, cofactor: default, gx: default, gy: default);
            var p1 = curve.CreatePoint(3, 6);
            var p2 = p1 * mul;
            if (p2.IsInfinity) {
                Assert.IsTrue(x == int.MaxValue && y == int.MaxValue);
            } else {
                Assert.AreEqual(new BigInteger(x), p2.X);
                Assert.AreEqual(new BigInteger(y), p2.Y);
            }
        }

        [Test]
        public void GetHexTest() {
            Assert.AreEqual("0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798", ECCurve.Secp256k1.G.GetHex());
            Assert.AreEqual("0479be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8", ECCurve.Secp256k1.G.GetHex(false));
        }

        [Test]
        public void InfinityGetHexTest() {
            Assert.AreEqual("00", ECPoint.Infinity.GetHex());
        }

    }
}
