using System.Numerics;
using NUnit.Framework;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(BigIntegerExt))]
    public class BigIntegerExtTests {

        [TestCase(-5, 23, 18)]
        [TestCase(5, 23, 5)]
        public void ModAbsTest(int a, int n, int res) {
            Assert.IsTrue(BigIntegerExt.ModAbs(a, n) == res);
        }

        [TestCase(7, 40832, 34999)]
        public void ModInverseTest(int a, int n, int expected) {
            var actual = BigIntegerExt.ModInverse(a, n);
            Assert.AreEqual(new BigInteger(expected), actual);
            Assert.IsTrue(BigIntegerExt.ModEqual(1, actual * a, n));
        }

        [TestCase(1, 24, 23, true)]
        [TestCase(2, 24, 23, false)]
        public void ModEqualTest(int a, int b, int n, bool res) {
            Assert.AreEqual(res, BigIntegerExt.ModEqual(a, b, n));
        }

        [TestCase(10, 3, 17, 9)]
        public void ModDivTest(int a, int b, int n, int res) {
            Assert.AreEqual(new BigInteger(res), BigIntegerExt.ModDiv(a, b, n));
        }

        [TestCase(10, 6, 17, 9)]
        public void ModMulTest(int a, int b, int n, int res) {
            Assert.AreEqual(new BigInteger(res), BigIntegerExt.ModMul(a, b, n));
        }

        [TestCase(5, 11, 4)]
        public void ModSqrtTest(int a, int n, int res) {
            Assert.AreEqual(new BigInteger(res), BigIntegerExt.ModSqrt(a, n));
        }

        [Test]
        public void EuclidExtendedTest() {
            var res = BigIntegerExt.EuclidExtended(51051, 21483);
            Assert.AreEqual(new BigInteger(51051), res.A);
            Assert.AreEqual(new BigInteger(21483), res.B);
            Assert.AreEqual(new BigInteger(8), res.X);
            Assert.AreEqual(new BigInteger(-19), res.Y);
            Assert.AreEqual(new BigInteger(231), res.Gcd);
        }

    }
}
