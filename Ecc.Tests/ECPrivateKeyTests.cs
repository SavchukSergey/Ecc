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

    }
}
