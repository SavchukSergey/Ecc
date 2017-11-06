using System.Security.Cryptography;
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

        [TestCase("0420319526ae7bb161eb650486c9c4ff80ab4f9e18d04d9da3651000d0ac335f169212d9fa5ab405e86985c20c8c1668612c73b441f1e10d8d728f844841455139")]
        [TestCase("04334180cfb8553b774d871c36be174171003cd13cb8325ad091b4f4b10934662f019ce8ef8369b3de4b097bf7ac1c06ce50c3c37452227b04d9efa29375e464b7")]
        [TestCase("04861529d088817d897efcc7233ff344a85c905bd2ae524b359eacd39537b5cb8ea6f491cb3c312d2f33f9494c0813870ca64579da6406722c0720823b59637f78")]
        public void CreatePointXYTest(string hex) {
            var curve = ECCurve.Secp256k1;
            var point = curve.CreatePoint(hex);
            Assert.IsTrue(point.Valid);
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

        [TestCase("0320319526ae7bb161eb650486c9c4ff80ab4f9e18d04d9da3651000d0ac335f16")]
        [TestCase("02334180cfb8553b774d871c36be174171003cd13cb8325ad091b4f4b10934662f")]
        [TestCase("02861529d088817d897efcc7233ff344a85c905bd2ae524b359eacd39537b5cb8e")]
        public void ParseKeyTest(string publicKeyHex) {
            var curve = ECCurve.Secp256k1;
            var publicKey = curve.CreatePublicKey(publicKeyHex);
            Assert.IsTrue(publicKey.Point.Valid);
        }

        [Test]
        public void TruncateHashTest() {
            var msg = "Hello World";
            var hash = SHA256.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(msg));
            var curve = ECCurve.Secp256k1;
            var res = curve.TruncateHash(hash);
            var actual = res.ToHexUnsigned(curve.KeySize8);
            Assert.AreEqual("6e149fadd977b2572ba3cd0bbf652cd690b1b7cf3317014a4020f40bd4a691a5", actual);
        }

    }
}
