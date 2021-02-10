using NUnit.Framework;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(BigUnsignedInteger))]
    public class BigUnsignedIntegerTests {

        [Test]
        public void AddTest() {
            var left = new BigUnsignedInteger(1234, 80);
            var right = new BigUnsignedInteger(4567, 80);
            left.Add(right);
            var native = left.ToNative().ToString();
            Assert.AreEqual("5801", native);
        }

    }
}
