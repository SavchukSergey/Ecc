using NUnit.Framework;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECPoint))]
    public class ECPointTests {

        [Test]
        public void AddTest() {
            var curve = new ECCurve {
                A = -1,
                B = 3,
                Modulus = 127
            };
            var p = curve.CreatePoint(16, 20);
            var q = curve.CreatePoint(41, 120);
            var r = p + q;
            Assert.IsTrue(r.X == 86);
            Assert.IsTrue(r.Y == 81);
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
