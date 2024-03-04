using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECPointByteCache))]
    public class ECPointByteCacheTests {

        [Test]
        public void GetTest() {
            var curve = ECCurve.Secp256k1;
            var cache = new ECPointByteCache(curve.G, curve.KeySize);
            ClassicAssert.AreEqual(ECPoint.Infinity.GetHex(), cache.Get(0, 0).GetHex());
            ClassicAssert.AreEqual(ECPoint.Infinity.GetHex(), cache.Get(1, 0).GetHex());
            ClassicAssert.AreEqual(ECPoint.Infinity.GetHex(), cache.Get(2, 0).GetHex());
            ClassicAssert.AreEqual(ECPoint.Infinity.GetHex(), cache.Get(31, 0).GetHex());

            ClassicAssert.AreEqual((curve.G * 1).GetHex(), cache.Get(0, 1).GetHex());
            ClassicAssert.AreEqual((curve.G * 256).GetHex(), cache.Get(1, 1).GetHex());
            ClassicAssert.AreEqual((curve.G * 65536).GetHex(), cache.Get(2, 1).GetHex());

            ClassicAssert.AreEqual((curve.G * 2).GetHex(), cache.Get(0, 2).GetHex());
            ClassicAssert.AreEqual((curve.G * 512).GetHex(), cache.Get(1, 2).GetHex());
            ClassicAssert.AreEqual((curve.G * 131072).GetHex(), cache.Get(2, 2).GetHex());

        }
    }
}
