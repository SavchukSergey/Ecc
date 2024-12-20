using System.Collections.Generic;
using Ecc.Math;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ecc.Tests.Math {
    public partial class BigInteger512Tests {

        [TestCaseSource(nameof(DivideCases512_512))]
        public void DivRem512Test(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger512.ParseHexUnsigned(leftHex);
            var right = BigInteger512.ParseHexUnsigned(rightHex);

            var res = BigInteger512.DivRem(left, right, out var remainder);

            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned(), "quotient");
            ClassicAssert.AreEqual(remHex, remainder.ToHexUnsigned(), "remainder");
        }

        [TestCaseSource(nameof(DivideCases512_256))]
        public void DivRem256Test(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger512.ParseHexUnsigned(leftHex);
            var right = BigInteger512.ParseHexUnsigned(rightHex);

            var remainder = new BigInteger512();
            var res = BigInteger512.DivRem(left, right.BiLow256, out remainder.BiLow256);

            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned(), "quotient");
            ClassicAssert.AreEqual(remHex, remainder.ToHexUnsigned(), "remainder");
        }

        [TestCaseSource(nameof(DivideCases512_128))]
        public void DivRem128Test(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger512.ParseHexUnsigned(leftHex);
            var right = BigInteger512.ParseHexUnsigned(rightHex);

            var remainder = new BigInteger512();
            var res = BigInteger512.DivRem(left, right.BiLow128, out remainder.BiLow128);

            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned(), "quotient");
            ClassicAssert.AreEqual(remHex, remainder.ToHexUnsigned(), "remainder");
        }

        public static IEnumerable<string[]> DivideCases512_512() {
            yield return [
                "ba42240ffb037f1a4c64905998460012cdf5f2e685baf6578d8eb71c34f932f81874bb356c05027886d076a3b80bad4d8122481d2b79b2135f4381e8326893e8",
                "0000000000e43220d5e39ee79e8098c2cf65094404f6aad8d12f8bbff8b4ca17b77183acf90bfe17c68ef34e89e9650b0317d8c7b01949c45df01ad807af17a5",
                "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000d0f3e6e05b",
                "0000000000909bbfb167d2810eea7984c0b2b96da890e5e2bceeceb97b360f82b856fc8637711d15aa00b57fba76933d25dd57c17343885a77c79f93653ccc41"
            ];
        }

        public static IEnumerable<string[]> DivideCases512_256() {
            yield return [
                "ba42240ffb037f1a4c64905998460012cdf5f2e685baf6578d8eb71c34f932f81874bb356c05027886d076a3b80bad4d8122481d2b79b2135f4381e8326893e8",
                "00000000000000000000000000000000000000000000000000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715487",
                "0000000000000000000000000000000000000000000000000000000000000001b08dc414ec2f90b39b1e1e738697a988498ac62c5b2bbe262d2fe70060f43c10",
                "00000000000000000000000000000000000000000000000000000000000000003c114b5f3ef2d59f01016dea10139935a356af32e798df81593e8b374ad7a778"
            ];
        }

        public static IEnumerable<string[]> DivideCases512_128() {
            yield return [
                "ba42240ffb037f1a4c64905998460012cdf5f2e685baf6578d8eb71c34f932f81874bb356c05027886d076a3b80bad4d8122481d2b79b2135f4381e8326893e8",
                "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000006e3be8abd2e089ed812475be9b51c3cf",
                "00000000000000000000000000000001b08dc414ec2f90b39b1e1e738697a98b6a6db58f40730ee88d6fd43f3587ed839ebc7633acec214d32ea5c2bf289a7d9",
                "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001fe5a4331e52789d0a67aa63bb969071"
            ];
        }

    }
}