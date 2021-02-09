using NUnit.Framework;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECPointCache))]
    public class ECPointCacheTests {

        [Test]
        public void Pow2Test() {
            var curve = ECCurve.Secp256k1;
            var privateKey = curve.CreateKeyPair();
            var cache = new ECPointCache(curve.G, curve.KeySize);
            var publicKey = curve.G;
            for (var i = 0; i < 13; i++) {
                var publicKeyCached = cache.Pow2(i);
                Assert.AreEqual(publicKey.GetHex(), publicKeyCached.GetHex());
                publicKey *= 2;
            }
        }
    }
}
