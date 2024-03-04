using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECPointBitCache))]
    public class ECPointBitCacheTests {

        [Test]
        public void Pow2Test() {
            var curve = ECCurve.Secp256k1;
            var cache = new ECPointBitCache(curve.G, curve.KeySize);
            var publicKey = curve.G;
            for (var i = 0; i < 13; i++) {
                var publicKeyCached = cache.Pow2(i);
                ClassicAssert.AreEqual(publicKey.GetHex(), publicKeyCached.GetHex());
                publicKey *= 2;
            }
        }
    }
}
