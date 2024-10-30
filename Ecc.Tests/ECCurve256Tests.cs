using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using Ecc.EC256;
using Ecc.Math;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECCurve256))]
    public class ECCurve256Tests {

        // [Test]
        public void GenerateCacheTest() {
            new ECHexInfo[] {
                Secp256k1Curve.HexInfo,
                NistP256Curve.HexInfo
            }
            .Select(curve => curve.Build256())
            .ToList()
            .ForEach(curve => {
                var cache = new ECPointByteCache256(curve.G, curve.KeySize);
                using var stream = File.OpenWrite($"{curve.Name}.cache.dat");
                cache.SaveTo(stream);
                stream.Flush();
                stream.Close();
            });
        }

        [Test]
        public void PeformanceTest() {
            var curve = ECCurve256.Secp256k1;
            var count = 10;
            var watch = new Stopwatch();
            var _ = curve.CreateKeyPair().PublicKey; // warm up
            var memStart = GC.GetAllocatedBytesForCurrentThread();
            watch.Start();
            for (var i = 0; i < count; i++) {
                var keyPair = curve.CreateKeyPair();
                var pubKey = keyPair.PublicKey;
                //ClassicAssert.NotNull(pubKey.Curve != null); //assert consumes 840 bytes
            }
            watch.Stop();
            var memEnd = GC.GetAllocatedBytesForCurrentThread();
            var kps = (double)count / watch.Elapsed.TotalSeconds;
            var bpk = (double)(memEnd - memStart) / (double)count;
            Console.WriteLine($"ec256 performance:");
            Console.WriteLine($"keys per second: {kps}");
            Console.WriteLine($"bytes per key  : {bpk}");
            Console.WriteLine("------------------------");
        }

        [Test]
        public void GetPublicKeyTest() {
            var curve = ECCurve256.Secp256k1;
            var privateKey = curve.CreateKeyPair();
            var publicKey = curve.GetPublicKey(privateKey.D);
            ClassicAssert.AreEqual(privateKey.PublicKey.Point.GetHex(), publicKey.Point.GetHex());
        }

        public void DocSample() {
            var curve = ECCurve256.Secp256k1;
            var keyPair = curve.CreateKeyPair();
            var msg = new BigInteger(4579485729345);
            var signature = keyPair.Sign(msg);
            var valid = keyPair.PublicKey.VerifySignature(msg, signature);
        }

        [Test]
        public void KeySizeTest() {
            ClassicAssert.AreEqual(256, ECCurve256.Secp256k1.KeySize);
        }

        [Test]
        public void OrderSizeTest() {
            ClassicAssert.AreEqual(256, ECCurve256.Secp256k1.OrderSize);
        }

        [TestCase("0420319526ae7bb161eb650486c9c4ff80ab4f9e18d04d9da3651000d0ac335f169212d9fa5ab405e86985c20c8c1668612c73b441f1e10d8d728f844841455139")]
        [TestCase("04334180cfb8553b774d871c36be174171003cd13cb8325ad091b4f4b10934662f019ce8ef8369b3de4b097bf7ac1c06ce50c3c37452227b04d9efa29375e464b7")]
        [TestCase("04861529d088817d897efcc7233ff344a85c905bd2ae524b359eacd39537b5cb8ea6f491cb3c312d2f33f9494c0813870ca64579da6406722c0720823b59637f78")]
        public void CreatePointXYTest(string hex) {
            var curve = ECCurve256.Secp256k1;
            var point = curve.CreatePoint(hex);
            ClassicAssert.IsTrue(point.Valid);
        }

        [TestCase("20319526ae7bb161eb650486c9c4ff80ab4f9e18d04d9da3651000d0ac335f16", true)]
        [TestCase("334180cfb8553b774d871c36be174171003cd13cb8325ad091b4f4b10934662f", false)]
        [TestCase("861529d088817d897efcc7233ff344a85c905bd2ae524b359eacd39537b5cb8e", false)]
        public void CreatePointXTest(string xHex, bool yOdd) {
            var x = BigInteger256.ParseHexUnsigned(xHex);
            var curve = ECCurve256.Secp256k1;
            var point = curve.CreatePoint(x, yOdd);
            ClassicAssert.IsTrue(point.Valid);
        }

        [TestCase(
            "870MB6gfuTJ4HtUnUvYMyJpr5eUZNP4Bk43bVdj3eAE",
            "MKBCTNIcKUSDii11ySs3526iDZ8AiTo7Tu6KPAqv7D4",
            "4Etl6SRW2YiLUrN5vfvVHuhp7x8PxltmWWlbbM4IFyM")]
        public void CreateBase64XTest(string dString, string xString, string yString) {
            var d = BigInteger256Ext.ParseBase64UrlUnsigned(dString);
            var x = BigInteger256Ext.ParseBase64UrlUnsigned(xString);
            var y = BigInteger256Ext.ParseBase64UrlUnsigned(yString);
            var curve = ECCurve256.NistP256;
            ClassicAssert.IsTrue(curve.Has(new ECPoint256(x, y, curve)));
            var point = curve.G * d;
            var actualX = point.X.ToBase64UrlUnsigned(curve.KeySize8);
            var actualY = point.Y.ToBase64UrlUnsigned(curve.KeySize8);
            ClassicAssert.AreEqual(xString, actualX);
            ClassicAssert.AreEqual(yString, actualY);
        }

        [TestCase("0320319526ae7bb161eb650486c9c4ff80ab4f9e18d04d9da3651000d0ac335f16")]
        [TestCase("02334180cfb8553b774d871c36be174171003cd13cb8325ad091b4f4b10934662f")]
        [TestCase("02861529d088817d897efcc7233ff344a85c905bd2ae524b359eacd39537b5cb8e")]
        public void ParseKeyTest(string publicKeyHex) {
            var curve = ECCurve256.Secp256k1;
            var publicKey = curve.CreatePublicKey(publicKeyHex);
            ClassicAssert.IsTrue(publicKey.Point.Valid);
        }

        [Test]
        public void TruncateHashTest() {
            var msg = "Hello World";
            var hash = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(msg));
            var curve = ECCurve256.Secp256k1;
            var res = curve.TruncateHash(hash);
            var actual = res.ToHexUnsigned();
            ClassicAssert.AreEqual("a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e", actual);
        }

        [Test]
        public void GetNamedCurvesTest() {
            foreach (var curve in ECCurve256.GetNamedCurves()) {
                ClassicAssert.IsTrue(curve.G.Valid, $"{curve.Name}");
            }
        }

    }
}
