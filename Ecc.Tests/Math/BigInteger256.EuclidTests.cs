using Ecc.Math;
using NUnit.Framework;

namespace Ecc.Tests.Math {
    public partial class BigInteger256Tests {

        [Test]
        public void EuclidExtendedTest() {
            var modulus = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var value = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");
            var ec = BigInteger256.EuclidExtended(in value, in modulus);
            AssertExt.AssertEquals("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f", ec.A, "A");
            AssertExt.AssertEquals("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", ec.B, "B");
            AssertExt.AssertEquals("78081ac91d31902fa64dc269971b21d8838347a2f67853964d4b2e2bcf8b32d0", ec.X, "X");
            AssertExt.AssertEquals("6a6c59bdd7a25d5fcd3b69d7f8b183194451df6a625bbecf68e7d86e194c21ac", ec.Y, "Y");
            // AssertExt.AssertEquals(new BigInteger256(1), ec.Gcd);
        }

    }
}