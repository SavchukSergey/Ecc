using NUnit.Framework;
using System.Numerics;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECPublicKey))]
    public class ECPublicKeyTests {

        [Test]
        public void VerifySignatureTest() {
            var curve = ECCurve.Secp256k1;
            var publicKey = curve.CreatePublicKey("0474938fcc21b40cd1fcb3e98df92c4239af59ef46f404a7d15ed659dcbdcda1326a7cd3040a023919418014d1b2c96b3b32467787938e82994b050d9968a8c5d2");
            var msg = BigIntegerExt.ParseHexUnsigned("7846e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
            var random = BigIntegerExt.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var signature = new ECSignature (
                BigIntegerExt.ParseHexUnsigned("2794dd08b1dfa958552bc37916515a3accb0527e40f9291d62cc4316047d24dd"),
                BigIntegerExt.ParseHexUnsigned("5dd1f95f962bb6871967dc17b22217100daa00a3756feb1e16be3e6936fd8594"),
                curve
            );
            var result = publicKey.VerifySignature(msg, signature);
            Assert.IsTrue(result);
        }

    }
}
