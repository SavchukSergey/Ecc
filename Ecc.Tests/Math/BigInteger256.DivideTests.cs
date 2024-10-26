using System.Collections.Generic;
using System.Numerics;
using Ecc.Math;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ecc.Tests.Math {
    public partial class BigInteger256Tests {

        [Test]
        public void ModDivTest() {
            var value = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var exponent = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");

            var result = value.ModDiv(exponent, modulus);

            Assert.That(result.ToHexUnsigned(), Is.EqualTo("182ff5254d4522b55f0ead2ebef2217bdd846824f3457dfdbbfc196ecc37453e"));
        }

        [TestCaseSource(nameof(DivideCases))]
        public void DivRemNativeTest(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger256.ParseHexUnsigned(leftHex);
            var right = BigInteger256.ParseHexUnsigned(rightHex);

            var res = BigInteger.DivRem(left.ToNative(), right.ToNative(), out var remainder);
            ClassicAssert.AreEqual(qHex, new BigInteger256(res).ToHexUnsigned());
            ClassicAssert.AreEqual(remHex, new BigInteger256(remainder).ToHexUnsigned());
        }

        [TestCaseSource(nameof(DivideCases))]
        public void DivRemTest(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger256.ParseHexUnsigned(leftHex);
            var right = BigInteger256.ParseHexUnsigned(rightHex);

            var res = BigInteger256.DivRem(left, right, out var remainder);
            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned(), "quotient");
            ClassicAssert.AreEqual(remHex, remainder.ToHexUnsigned(), "remainder");

            var nativeRes = BigInteger.DivRem(left.ToNative(), right.ToNative(), out var nativeRemainder);
            ClassicAssert.AreEqual(qHex, new BigInteger256(nativeRes).ToHexUnsigned(), "native.quotient");
            ClassicAssert.AreEqual(remHex, new BigInteger256(nativeRemainder).ToHexUnsigned(), "native.remainder");
        }

        [TestCaseSource(nameof(DivideCases))]
        public void DivRemFullBitsTest(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger256.ParseHexUnsigned(leftHex);
            var right = BigInteger256.ParseHexUnsigned(rightHex);

            var res = BigInteger256.DivRemFullBits(left, right, out var remainder);
            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned());
            ClassicAssert.AreEqual(remHex, remainder.ToHexUnsigned());
        }

        [TestCaseSource(nameof(DivideCases))]
        public void DivRemBitsTest(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger256.ParseHexUnsigned(leftHex);
            var right = BigInteger256.ParseHexUnsigned(rightHex);

            var res = BigInteger256.DivRemBits(left, right, out var remainder);
            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned());
            ClassicAssert.AreEqual(remHex, remainder.ToHexUnsigned());
        }

        [TestCaseSource(nameof(DivideCases))]
        public void DivRemNewtonTest(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger256.ParseHexUnsigned(leftHex);
            var right = BigInteger256.ParseHexUnsigned(rightHex);

            var res = BigInteger256.DivRemNewton(left, right, out var remainder);
            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned());
            ClassicAssert.AreEqual(remHex, remainder.ToHexUnsigned());
        }

        public static IEnumerable<string[]> DivideCases() {
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000000000000000000000000000000000000000000000000000000000006",
                "223d2bb3ad4647eb58fe9bbec7364ecaf8b3a977d6372d39805b3fc21efd0ffb",
                "0000000000000000000000000000000000000000000000000000000000000003"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "000000000000000000000000000000000000000000000000000000000000006e",
                "01de19a80e1b1b3212d160ee7a9340ce8fe49039bc8ea5603a3385ce144ef790",
                "0000000000000000000000000000000000000000000000000000000000000005"
            ];

            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "000000000000000000000000000000006e3be8abd2e089ed812475be9b51c3cf",
                "00000000000000000000000000000001dd15d2c949aeda9ea5c7f77e1c7d2c3b",
                "000000000000000000000000000000001294f2aac703fc89ac5cb7b3a05bab30"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "00000000000000000000000000000006e3be8abd2e089ed812475be9b51c3cfc",
                "000000000000000000000000000000001dd15d2c949aeda9ea5c7f77e1c7d2c3",
                "00000000000000000000000000000003695793f5df66c5c73c97cd45b78133f1"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000000000000000000000000006e3be8abd2e089ed812475be9b51c3cfcc",
                "0000000000000000000000000000000001dd15d2c949aeda9ea5c7f77e1c7d2c",
                "00000000000000000000000000000017fe362e4bfa0c701003a88168ed800cd5"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "000000000000000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1",
                "00000000000000000000000000000000001dd15d2c949aeda9ea5c7f77e1c7d2",
                "00000000000000000000000000000542cd006ad153eef7300f4314314acc0293"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "00000000000000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a",
                "000000000000000000000000000000000001dd15d2c949aeda9ea5c7f77e1c7d",
                "0000000000000000000000000000130a4a034253294fc68013c86dcbd858df33"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000000000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a0",
                "0000000000000000000000000000000000001dd15d2c949aeda9ea5c7f77e1c7",
                "0000000000000000000000000005ac151abcf7ba2a5f555a0e7650f2c9e63c85"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "000000000000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04",
                "00000000000000000000000000000000000001dd15d2c949aeda9ea5c7f77e1c",
                "0000000000000000000000000035e64ae5e932822afcb026d66fca2154f18f75"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "00000000000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04f",
                "000000000000000000000000000000000000001dd15d2c949aeda9ea5c7f77e1",
                "0000000000000000000000000560b532f3cbb73a0894c8f6b1caaa93a70ac176"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fa",
                "0000000000000000000000000000000000000001dd15d2c949aeda9ea5c7f77e",
                "0000000000000000000000000c4473bdb0f9bfc63dccd475baab3c9e29fcb6d9"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "000000000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04faf",
                "00000000000000000000000000000000000000001dd15d2c949aeda9ea5c7f77",
                "000000000000000000000006138b2d2339414ac18e86cf458410050697fd848c"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "00000000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa",
                "000000000000000000000000000000000000000001dd15d2c949aeda9ea5c7f7",
                "0000000000000000000000364dc0f84f7b7da2a9fbd7786d9af4db65b0e5e2af"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2",
                "0000000000000000000000000000000000000000001dd15d2c949aeda9ea5c7f",
                "000000000000000000000339f11dab139f432131fad40bf27e26541f13340687"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "000000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2d",
                "00000000000000000000000000000000000000000001dd15d2c949aeda9ea5c7",
                "000000000000000000006a921b3ec0c62091ca44291e8373718065965dd0e5ea"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "00000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2dd",
                "000000000000000000000000000000000000000000001dd15d2c949aeda9ea5c",
                "000000000000000000036e3577f184e9e610524361533f0e10eb6e6e6f0cd679"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb",
                "0000000000000000000000000000000000000000000001dd15d2c949aeda9ea5",
                "000000000000000000561b23f8cfad5158312d9bb047a7e0fcb5dbc7a7e3a7be"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "000000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6",
                "00000000000000000000000000000000000000000000001dd15d2c949aeda9ea",
                "0000000000000000027d46af53ee1002fbb6e3e86950400123835264ed349189"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "00000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6c",
                "000000000000000000000000000000000000000000000001dd15d2c949aeda9e",
                "00000000000000004762b81ab7ba66376c6dad7f8a635a4ca60db412555efb3d"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca",
                "0000000000000000000000000000000000000000000000001dd15d2c949aeda9",
                "00000000000000064ea971804001f1347c6c1dec08dc0fa8a54c560e35d3b28b"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "000000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6",
                "00000000000000000000000000000000000000000000000001dd15d2c949aeda",
                "0000000000000044505c5226de4f86cd20ee592366da348b68c0a04a37e50689"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "00000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca68",
                "000000000000000000000000000000000000000000000000001dd15d2c949aed",
                "0000000000000492a7730863a3b2cdd88d87cb34988052846c03e3269dd26d9d"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca686",
                "0000000000000000000000000000000000000000000000000001dd15d2c949ae",
                "0000000000005e23547ea3ba13c3c6c62d32a9670799285990049fe8ef62fad1"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "000000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869",
                "00000000000000000000000000000000000000000000000000001dd15d2c949a",
                "000000000006656a0de42c019ec0d6c49d9f27dfbcf651c5d5bd420ea8c2dcbb"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "00000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869b",
                "000000000000000000000000000000000000000000000000000001dd15d2c949",
                "00000000004b4adb7947f857d3318d8e34c03afa1ed5e208f2998a37dc9c4ab2"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf",
                "0000000000000000000000000000000000000000000000000000001dd15d2c94",
                "00000000042b660983b1dd312cbbd5b1e8361adc6d02cc35c565252f6a016979"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "000000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf2",
                "00000000000000000000000000000000000000000000000000000001dd15d2c9",
                "000000001fba60347869ffac8d04f3218f0a8bd060094d7483f09c06d9776ae3"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "00000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf27",
                "000000000000000000000000000000000000000000000000000000001dd15d2c",
                "00000003ffd58e3ee24ed906174d16d504ea6e1e8cf37a474f8d52d7e3795a31"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272",
                "0000000000000000000000000000000000000000000000000000000001dd15d2",
                "00000056acc40f1d0ab64b26f2a565c9823d49f7a0773682f1f1e454c4b42461"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "000006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf2727",
                "00000000000000000000000000000000000000000000000000000000001dd15d",
                "00000133249566c2cbca26293b90e30025c4e98fd4812c774da8bda1d11b4fba"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "00006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf27271",
                "000000000000000000000000000000000000000000000000000000000001dd15",
                "00005ac3d1a102193bdb1f16db3bc13294ddbf64f88d17bc8ff104ef2fce6fa0"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715",
                "0000000000000000000000000000000000000000000000000000000000001dd1",
                "000281ef5cfc207bed7ea4cd27f4c9cb67ecbbe71173faa1d91045903b8a16c0"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf2727154",
                "00000000000000000000000000000000000000000000000000000000000001dd",
                "000965ade7b94e848c56b71483de7ee7a4e97d8761239d7f8fdaadf9fab13661"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "06e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf27271548",
                "000000000000000000000000000000000000000000000000000000000000001d",
                "05a2707ea16eb5859be5910f31c1a5d93246cfc81fd7f1a4144f83e54a80f6bd"
            ];
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "6e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715487",
                "0000000000000000000000000000000000000000000000000000000000000001",
                "5f331d8a3cc5259694d330ba0ff414f2081bf3d40b1d33ec5b9ce29a477d0b5e"
            ];

            yield return [
                "0cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe",
                "6e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715487",
                "0000000000000000000000000000000000000000000000000000000000000000",
                "0cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe"
            ];
            yield return [
                "0cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe",
                "06e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf27271548",
                "0000000000000000000000000000000000000000000000000000000000000001",
                "05f331d8a3cc5259694d330ba0ff414f2081bf3d40b1d33ec5b9ce29a477d0b6"
            ];
            yield return [
                "197d277f9200e92c608b4ee01b89d105ecb7a1bc492f7ada274a979ca1589194",
                "01cbd1cb1e58064c11814b583f061e9d431cca8e4cea1313449bf97c840ae0ab",
                "000000000000000000000000000000000000000000000000000000000000000e",
                "0057ae63e93091036b79300ca934246c41248df414626fcc66c2f2cd68c0483a"
            ];
            yield return [
                "000000000000000119088c1de6ce815ff2846ea135169bec3a9dbd58f7d62900",
                "000000000000000050bf6b700c9d1160fbe38d2ebff9b62699075e141dfb294d",
                "0000000000000000000000000000000000000000000000000000000000000003",
                "000000000000000026ca49cdc0f74d3cfed9c714f52979786f87a31c9de4ad19"
            ];
            yield return [
                "000000000000000000000000021f2aaa3dc041cdc12f88278c3ba95e33da9e77",
                "000000000000000000000000016ca6eaedbec18da0d5c491da655c2e0b1a0c2f",
                "0000000000000000000000000000000000000000000000000000000000000001",
                "00000000000000000000000000b283bf500180402059c395b1d64d3028c09248"
            ];
        }

    }
}