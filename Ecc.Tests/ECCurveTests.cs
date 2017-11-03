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

        [Test, Ignore("to implement")]
        public void CreatePointXYTest() {
            //todo: implement
            throw new System.NotImplementedException();
        }

        [TestCase("20319526ae7bb161eb650486c9c4ff80ab4f9e18d04d9da3651000d0ac335f16", true)]
        [TestCase("334180cfb8553b774d871c36be174171003cd13cb8325ad091b4f4b10934662f", false)]
        [TestCase("861529d088817d897efcc7233ff344a85c905bd2ae524b359eacd39537b5cb8e", false)]
        public void CreatePointXTest(string xHex, bool yOdd) {
            var x = BigIntegerExt.ParseHexUnsigned(xHex);
            var curve = ECCurve.Secp256k1;
            var point = curve.CreatePoint(x, yOdd);
            Assert.IsTrue(point.Valid);
        }

        [Test, Ignore("to implement")]
        public void TruncateHashTest() {
            //todo: implement
            throw new System.NotImplementedException();
        }

    }
}
