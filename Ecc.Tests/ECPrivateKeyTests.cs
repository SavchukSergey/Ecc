using NUnit.Framework;
using System.Numerics;

namespace Ecc.Tests {
    [TestFixture]
    public class ECPrivateKeyTests {

        [Test]
        public void CreateTest() {
            var curve = new ECCurve {
                A = 2,
                B = 3,
                Modulus = 97,
                Order = 100
            };
            var privateKey = ECPrivateKey.Create(curve);
            Assert.IsNotNull(privateKey);
        }

        [Test]
        public void SignTest() {
            foreach (var curve in ECCurve.GetNamedCurves()) {
                var privateKey = ECPrivateKey.Create(curve);
                var msg = new BigInteger(4579485729345);
                var signature = privateKey.Sign(msg);
                var valid = privateKey.PublicKey.VerifySignature(msg, signature);
                Assert.IsTrue(valid, $"curve {curve.Name}");
            }
        }

        [TestCase("42c6d7b9570c75e3778b3ac7bfd851198816da1dd39ce420f16378c9418b8a58", "0320319526ae7bb161eb650486c9c4ff80ab4f9e18d04d9da3651000d0ac335f16")]
        [TestCase("754119b222e208b4c24936bd6aae22b3f760956f905103270705e5257e467344", "02334180cfb8553b774d871c36be174171003cd13cb8325ad091b4f4b10934662f")]
        [TestCase("9ada5ef64bff005b95afb0acb5f7d51df1f42ed5435c09ab7eb4ed9b6eb08572", "02861529d088817d897efcc7233ff344a85c905bd2ae524b359eacd39537b5cb8e")]
        public void PublicKeyTest(string privateKeyHex, string publicKeyHex) {
            var curve = ECCurve.Secp256k1;
            var privateKey = curve.ParsePrivateKeyHex(privateKeyHex);
            var publicKey = privateKey.PublicKey;
            Assert.AreEqual(publicKeyHex, publicKey.GetCompressedHex());
        }

    }
}
