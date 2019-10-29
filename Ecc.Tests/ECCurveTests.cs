using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using NUnit.Framework;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECCurve))]
    public class ECCurveTests {

        [Test]
        public void PeformanceTest() {
            var curve = ECCurve.Secp256k1;
            var count = 100;
            var watch = new Stopwatch();
            var memStart = GC.GetTotalAllocatedBytes();
            watch.Start();
            for (var i = 0; i < count; i++) {
                var keyPair = curve.CreateKeyPair();
                var pubKey = keyPair.PublicKey;
                Assert.NotNull(pubKey);
            }
            watch.Stop();
            var memEnd = GC.GetTotalAllocatedBytes();
            var kps = (double)count / watch.Elapsed.TotalSeconds;
            var bpk = (double)(memEnd - memStart) / (double)count;
            Console.WriteLine($"keys per second: {kps}");
            Console.WriteLine($"bytes per key  : {bpk}");
        }

        [Test]
        public void GetPublicKeyTest() {
            var curve = ECCurve.Secp256k1;
            var privateKey = curve.CreateKeyPair();
            var publicKey = curve.GetPublicKey(privateKey.D);
            Assert.AreEqual(privateKey.PublicKey.Point.GetHex(), publicKey.Point.GetHex());
        }

        public void DocSample() {
            var curve = ECCurve.Secp256k1;
            var keyPair = curve.CreateKeyPair();
            var msg = new BigInteger(4579485729345);
            var signature = keyPair.Sign(msg);
            var valid = keyPair.PublicKey.VerifySignature(msg, signature);
        }

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

        [TestCase(
            "870MB6gfuTJ4HtUnUvYMyJpr5eUZNP4Bk43bVdj3eAE",
            "MKBCTNIcKUSDii11ySs3526iDZ8AiTo7Tu6KPAqv7D4",
            "4Etl6SRW2YiLUrN5vfvVHuhp7x8PxltmWWlbbM4IFyM")]
        public void CreateBase64XTest(string dString, string xString, string yString) {
            var d = BigIntegerExt.ParseBase64UrlUnsigned(dString);
            var x = BigIntegerExt.ParseBase64UrlUnsigned(xString);
            var y = BigIntegerExt.ParseBase64UrlUnsigned(yString);
            var curve = ECCurve.NistP256;
            Assert.IsTrue(curve.Has(new ECPoint(x, y, curve)));
            var point = curve.G * d;
            var mod = curve.Modulus.ToBase64UrlUnsigned(curve.KeySize8);
            var actualX = point.X.ToBase64UrlUnsigned(curve.KeySize8);
            var actualY = point.Y.ToBase64UrlUnsigned(curve.KeySize8);
            Assert.AreEqual(xString, actualX);
            Assert.AreEqual(yString, actualY);
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
            Assert.AreEqual("a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e", actual);
        }

        [Test]
        public void GetNamedCurvesTest() {
            foreach (var curve in ECCurve.GetNamedCurves()) {
                Assert.IsTrue(curve.G.Valid, $"{curve.Name}");
            }
        }

    }
}
