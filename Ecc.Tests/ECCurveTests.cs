using NUnit.Framework;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECCurve))]
    public class ECCurveTests {

        [Test]
        public void KeySizeTest() {
            Assert.AreEqual(256, ECCurve.Secp256k1.KeySize);
            Assert.AreEqual(384, ECCurve.Secp384r1.KeySize);
            Assert.AreEqual(521, ECCurve.Secp521r1.KeySize);
        }

        [Test]
        public void OrderSizeTest() {
            Assert.AreEqual(256, ECCurve.Secp256k1.OrderSize);
            Assert.AreEqual(384, ECCurve.Secp384r1.OrderSize);
            Assert.AreEqual(521, ECCurve.Secp521r1.OrderSize);
        }

    }
}
