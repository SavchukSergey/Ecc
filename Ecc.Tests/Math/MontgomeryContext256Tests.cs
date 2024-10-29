using System.Collections.Generic;
using Ecc.Math;
using NUnit.Framework;

namespace Ecc.Tests.Math {
    [TestFixture]
    public partial class MontgomeryContext256Tests {

        [TestCaseSource(nameof(ConvertCases))]
        public void ToMontgomeryTest(string realValue, string montValue) {
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");
            var ctx = new MontgomeryContext256(modulus);
            AssertExt.AssertEquals(
                montValue,
                ctx.ToMontgomery(BigInteger256.ParseHexUnsigned(realValue))
            );
        }

        [TestCaseSource(nameof(ConvertCases))]
        public void Reduce256Test(string realValue, string montValue) {
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");
            var ctx = new MontgomeryContext256(modulus);
            AssertExt.AssertEquals(
                realValue,
                ctx.Reduce(BigInteger256.ParseHexUnsigned(montValue))
            );
        }

        [Test]
        public void Reduce512Test() {
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");
            var ctx = new MontgomeryContext256(modulus);
            var montValue = BigInteger512.ParseHexUnsigned("070e8ec4d1e089f01e6ec9963bdbb70f04e9d0fbb2942a281be9ea4eb9f540eee346335f6a9a829c3d5db39ad21dfb6816f18830c16a03cdf77821220eb1aa1b");
            var realValue = ctx.Reduce(montValue);
            AssertExt.AssertEquals("F4D2F4094AA042FEB5580B848904CC355D3E150CAFEDED81F9C164242050FE4A", realValue);
        }

        [TestCase("145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b", "0000000000000000000000000000000000000000000000000000000000000000", "0000000000000000000000000000000000000000000000000000000000000000")]
        [TestCase("145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b", "00000000000000000000000000000000000000000000000000000001000003d1", "145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b")]
        [TestCase("145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b", "58BA10A3A23714ED6AE6B1E13EF65E16FBE0EF9208EED8351D78908BEF3FB701", "F4D2F4094AA042FEB5580B848904CC355D3E150CAFEDED81F9C164242050FE4A")]
        [TestCase("145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b", "145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b", "FA06E3B927A10AA9C448C6F1A36F66DDE5455690C05EC4C7ED0282C394612022")]
        [TestCase("FA06E3B927A10AA9C448C6F1A36F66DDE5455690C05EC4C7ED0282C394612022", "FA06E3B927A10AA9C448C6F1A36F66DDE5455690C05EC4C7ED0282C394612022", "16F2DA2A2F024836B8D0A5B8703D5437869F901B1A93506A1CE6F7F210C1081B")]
        public void MulTest(string leftMontHex, string rightMontHex, string expectedMontHex) {
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");
            var ctx = new MontgomeryContext256(modulus);
            var leftMont = BigInteger256.ParseHexUnsigned(leftMontHex);
            var rightMont = BigInteger256.ParseHexUnsigned(rightMontHex);
            var result1Mont = ctx.ModMul(leftMont, rightMont);
            AssertExt.AssertEquals(expectedMontHex, result1Mont);
            var result2Mont = ctx.ModMul(rightMont, leftMont);
            AssertExt.AssertEquals(expectedMontHex, result2Mont);

            //recheck
            var leftReal = ctx.Reduce(leftMont);
            var rightReal = ctx.Reduce(rightMont);
            var resultReal = leftReal.ModMul(rightReal, modulus);

            AssertExt.AssertEquals(resultReal, ctx.Reduce(result1Mont), "Recheck");
        }

        [TestCase("0000000000000000000000000000000000000000000000000000000000000000", "0000000000000000000000000000000000000000000000000000000000000000")]
        [TestCase("00000000000000000000000000000000000000000000000000000001000003d1", "00000000000000000000000000000000000000000000000000000001000003d1")]
        [TestCase("145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b", "FA06E3B927A10AA9C448C6F1A36F66DDE5455690C05EC4C7ED0282C394612022")]
        [TestCase("FA06E3B927A10AA9C448C6F1A36F66DDE5455690C05EC4C7ED0282C394612022", "16F2DA2A2F024836B8D0A5B8703D5437869F901B1A93506A1CE6F7F210C1081B")]
        public void SquareTest(string valueMontHex, string expectedMontHex) {
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");
            var ctx = new MontgomeryContext256(modulus);
            var valueMont = BigInteger256.ParseHexUnsigned(valueMontHex);
            var resultMont = ctx.ModSquare(valueMont);
            AssertExt.AssertEquals(expectedMontHex, resultMont);

            //recheck
            var valueReal = ctx.Reduce(valueMont);
            var resultReal = valueReal.ModSquare(modulus);

            AssertExt.AssertEquals(resultReal, ctx.Reduce(resultMont), "Recheck");
        }

        [TestCase("145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b", "0000000000000000000000000000000000000000000000000000000000000000", "00000000000000000000000000000000000000000000000000000001000003D1")]
        [TestCase("145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b", "0000000000000000000000000000000000000000000000000000000000000001", "145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b")]
        [TestCase("145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b", "0000000000000000000000000000000000000000000000000000000000000002", "FA06E3B927A10AA9C448C6F1A36F66DDE5455690C05EC4C7ED0282C394612022")]
        [TestCase("145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b", "0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715", "302E045E0C7F80B5552D059539516A8511999C7DF142ABF66F41F8B9E734061F")]
        public void PowTest(string valueMontHex, string expHex, string expectedMontHex) {
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");
            var ctx = new MontgomeryContext256(modulus);
            var valueMont = BigInteger256.ParseHexUnsigned(valueMontHex);
            var exp = BigInteger256.ParseHexUnsigned(expHex);
            var resultMont = ctx.ModPow(valueMont, exp);
            AssertExt.AssertEquals(expectedMontHex, resultMont);

            //recheck
            var valueReal = ctx.Reduce(valueMont);
            var resultReal = valueReal.ModPow(exp, modulus);

            AssertExt.AssertEquals(resultReal, ctx.Reduce(resultMont), "Recheck");
        }

        public static IEnumerable<string[]> ConvertCases() {
            yield return [
                "0000000000000000000000000000000000000000000000000000000000000000",
                "0000000000000000000000000000000000000000000000000000000000000000",
            ];
            yield return [
                "0000000000000000000000000000000000000000000000000000000000000001",
                "00000000000000000000000000000000000000000000000000000001000003d1",
            ];
            yield return [
                "0000000000000000000000000000000023e0715c2534cec45e6ac5894b97fccd",
                "00000000000000000000000023e071e510c56f6e5cf3e0f5a113ddc67f0bca5d",
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "145c63d5cd4a7d9081682b4779c63489e7459d6c3599113eb0d3634c9b7e5d1b",
            ];
            yield return [
                "223d2bb3ad4647eb58fe9bbec7364ecaf8b3a977d6372d39805b3fc21efd0ffb",
                "58BA10A3A23714ED6AE6B1E13EF65E16FBE0EF9208EED8351D78908BEF3FB701",
            ];
            yield return [
                "42600ED6A43A52E08809F817874C39B92F1C80789177C8E985C82A4A2677EE57",
                "F4D2F4094AA042FEB5580B848904CC355D3E150CAFEDED81F9C164242050FE4A",
            ];
        }
    }
}