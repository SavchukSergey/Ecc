using NUnit.Framework;
using System.Numerics;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECPublicKey))]
    public class ECPublicKeyTests {

        [TestCase("0320319526ae7bb161eb650486c9c4ff80ab4f9e18d04d9da3651000d0ac335f16")]
        [TestCase("02334180cfb8553b774d871c36be174171003cd13cb8325ad091b4f4b10934662f")]
        [TestCase("02861529d088817d897efcc7233ff344a85c905bd2ae524b359eacd39537b5cb8e")]
        public void ParseKeyTest(string publicKeyHex) {
            var curve = ECCurve.Secp256k1;
            var publicKey = ECPublicKey.ParseHex(publicKeyHex, curve);
            Assert.IsTrue(publicKey.Point.Valid);
        }

    }
}
