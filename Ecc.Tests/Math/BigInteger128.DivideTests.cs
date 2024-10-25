using System.Collections.Generic;
using System.Numerics;
using Ecc.Math;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ecc.Tests.Math {
    public partial class BigInteger128Tests {

        [TestCaseSource(nameof(DivideCases))]
        public void DivRem_128_128Test(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger128.ParseHexUnsigned(leftHex);
            var right = BigInteger128.ParseHexUnsigned(rightHex);

            var res = BigInteger128.DivRem(left, right, out var remainder);
            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned());
            ClassicAssert.AreEqual(remHex, remainder.ToHexUnsigned());

            var resN = BigInteger.DivRem(left.ToNative(), right.ToNative(), out var remainderN);
            ClassicAssert.AreEqual(qHex, new BigInteger128(resN).ToHexUnsigned());
            ClassicAssert.AreEqual(remHex, new BigInteger128(remainderN).ToHexUnsigned());
        }

        [Test]
        public void DivRem_128_64_BigQTest() {
            var res = BigInteger128.DivRem(BigInteger128.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1"), 6ul, out ulong remainder);
            ClassicAssert.AreEqual("223d2bb3ad4647eb58fe9bbec7364eca", res.ToHexUnsigned());
            ClassicAssert.AreEqual(5, remainder);
        }

        public static IEnumerable<string[]> DivideCases() {
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "00000000000000000000000000000006",
                "223d2bb3ad4647eb58fe9bbec7364eca",
                "00000000000000000000000000000005"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "0000000000000000000000000000006e",
                "01de19a80e1b1b3212d160ee7a9340ce",
                "0000000000000000000000000000003d"
            ];

            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "000000000000000000000000000006e3",
                "001dd4962cbb8e6706d65222eb0ac8f1",
                "0000000000000000000000000000050e"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "00000000000000000000000000006e3b",
                "0001dd19c1cdb866d5ae2bd069bf8749",
                "00000000000000000000000000004cee"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "0000000000000000000000000006e3be",
                "00001dd15f8508b312d1fc883c9a748c",
                "000000000000000000000000000334d9"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "000000000000000000000000006e3be8",
                "000001dd15d5b0ede0a0f9fc6f86fa28",
                "000000000000000000000000001bec81"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "00000000000000000000000006e3be8a",
                "0000001dd15d2fc75cc84474a17f4f02",
                "000000000000000000000000053bc5ad"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "0000000000000000000000006e3be8ab",
                "00000001dd15d2ccda5838f4bc8c786e",
                "00000000000000000000000066f3b747"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "000000000000000000000006e3be8abd",
                "000000001dd15d2c956228b0abd80b30",
                "000000000000000000000003791db651"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "00000000000000000000006e3be8abd2",
                "0000000001dd15d2c94d7aa472669df6",
                "00000000000000000000000d6ea4f2f5"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "0000000000000000000006e3be8abd2e",
                "00000000001dd15d2c949b12f8fc0b40",
                "000000000000000000000546492d9341"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "000000000000000000006e3be8abd2e0",
                "000000000001dd15d2c949b12f8fc0b4",
                "000000000000000000000546492d9341"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "00000000000000000006e3be8abd2e08",
                "0000000000001dd15d2c949af05961a9",
                "000000000000000000063360e5296d79"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "0000000000000000006e3be8abd2e089",
                "00000000000001dd15d2c949aedea28c",
                "00000000000000000067b8f26d0e5bd5"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "000000000000000006e3be8abd2e089e",
                "000000000000001dd15d2c949aedad91",
                "00000000000000000377ffe6621a3143"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "00000000000000006e3be8abd2e089ed",
                "0000000000000001dd15d2c949aedaa0",
                "00000000000000005b96d87a5565d2a1"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "0000000000000006e3be8abd2e089ed8",
                "00000000000000001dd15d2c949aeda9",
                "000000000000000650ca79d2de970429"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "000000000000006e3be8abd2e089ed81",
                "000000000000000001dd15d2c949aeda",
                "000000000000004450a044a6b39aeae7"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "00000000000006e3be8abd2e089ed812",
                "0000000000000000001dd15d2c949aed",
                "0000000000000492a77b58291fd4fc17"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "0000000000006e3be8abd2e089ed8124",
                "00000000000000000001dd15d2c949ae",
                "0000000000005e23547f7f2844c0ce49"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "000000000006e3be8abd2e089ed81247",
                "000000000000000000001dd15d2c949a",
                "000000000006656a0de436b64385ce0b"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "00000000006e3be8abd2e089ed812475",
                "0000000000000000000001dd15d2c949",
                "00000000004b4adb7947f9bb0ad89664"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "0000000006e3be8abd2e089ed812475b",
                "00000000000000000000001dd15d2c94",
                "00000000042b660983b1dd4c6561f425"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "000000006e3be8abd2e089ed812475be",
                "000000000000000000000001dd15d2c9",
                "000000001fba60347869ffadae798a93"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "00000006e3be8abd2e089ed812475be9",
                "0000000000000000000000001dd15d2c",
                "00000003ffd58e3ee24ed9062c6567b5"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "0000006e3be8abd2e089ed812475be9b",
                "00000000000000000000000001dd15d2",
                "00000056acc40f1d0ab64b26f33dc69b"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "000006e3be8abd2e089ed812475be9b5",
                "000000000000000000000000001dd15d",
                "00000133249566c2cbca26293b942d00"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "00006e3be8abd2e089ed812475be9b51",
                "0000000000000000000000000001dd15",
                "00005ac3d1a102193bdb1f16db3d2e1c"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "0006e3be8abd2e089ed812475be9b51c",
                "00000000000000000000000000001dd1",
                "000281ef5cfc207bed7ea4cd27f4d0e5"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "006e3be8abd2e089ed812475be9b51c3",
                "000000000000000000000000000001dd",
                "000965ade7b94e848c56b71483de806a"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "06e3be8abd2e089ed812475be9b51c3c",
                "0000000000000000000000000000001d",
                "05a2707ea16eb5859be5910f31c1a5f5"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "6e3be8abd2e089ed812475be9b51c3cf",
                "00000000000000000000000000000001",
                "5f331d8a3cc5259694d330ba0ff414f2"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1",
                "e3be8abd2e089ed812475be9b51c3cfc",
                "00000000000000000000000000000000",
                "cd6f06360fa5af8415f7a678ab45d8c1"
            ];
        }

    }
}