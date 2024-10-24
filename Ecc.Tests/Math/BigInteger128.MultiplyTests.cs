using Ecc.Math;
using NUnit.Framework;

namespace Ecc.Tests.Math {
    public partial class BigInteger128Tests {

        [Test]
        public void Mul64Test() {
            var left = BigInteger128.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1");
            var right = 0xb6ca6869bf272715UL;

            var result = left * right;

            Assert.That(result.ToHexUnsigned(), Is.EqualTo("92af5bcf4c3ce6f52c10cadf9306bd65c9d68d82b5272ed5"));
        }

    }
}