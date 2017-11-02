using NUnit.Framework;

namespace Ecc.Tests {
    [TestFixture]
    public class ECCurveTests {

        [Test]
        public void KeySizeTest() {
            Assert.AreEqual(256, ECCurve.Secp256k1.KeySize);
            Assert.AreEqual(384, ECCurve.Secp384r1.KeySize);
            Assert.AreEqual(521, ECCurve.Secp521r1.KeySize);
        }

        [TestCase(1, 3, 6)]
        [TestCase(2, 80, 10)]
        [TestCase(3, 80, 87)]
        [TestCase(4, 3, 91)]
        [TestCase(5, int.MaxValue, int.MaxValue)]
        [TestCase(6, 3, 6)]
        public void MulTest(int mul, int x, int y) {
            var curve = new ECCurve {
                A = 2,
                B = 3,
                Modulus = 97
            };
            var p1 = curve.CreatePoint(3, 6);
            var p2 = p1 * mul;
            if (p2 == null) {
                Assert.IsTrue(x == int.MaxValue && y == int.MaxValue);
            } else {
                Assert.IsTrue(p2.X == x);
                Assert.IsTrue(p2.Y == y);
            }
        }

    }
}
