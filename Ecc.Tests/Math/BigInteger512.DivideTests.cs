using System.Collections.Generic;
using System.Numerics;
using Ecc.Math;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ecc.Tests.Math {
    public partial class BigInteger512Tests {

        [TestCaseSource(nameof(DivideCases512_256))]
        public void DivRemGuess256Test(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger512.ParseHexUnsigned(leftHex);
            var right = BigInteger512.ParseHexUnsigned(rightHex);

            var res = BigInteger512.DivRemGuess(left, right.Low, out var remainder);

            var nativeRes = BigInteger.DivRem(left.ToNative(), right.ToNative(), out var nativeRemainder);
            ClassicAssert.AreEqual(qHex, new BigInteger512(nativeRes).ToHexUnsigned(), "native.quotient");
            ClassicAssert.AreEqual(remHex, new BigInteger512(nativeRemainder).ToHexUnsigned(), "native.remainder");

            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned(), "quotient");
            ClassicAssert.AreEqual(remHex, remainder.ToHexUnsigned(), "remainder");
        }

        [TestCaseSource(nameof(DivideCases512_128))]
        public void DivRemGuess128Test(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger512.ParseHexUnsigned(leftHex);
            var right = BigInteger512.ParseHexUnsigned(rightHex);

            var res = BigInteger512.DivRemGuess(left, right.Low.BiLow, out var remainder);

            var nativeRes = BigInteger.DivRem(left.ToNative(), right.ToNative(), out var nativeRemainder);
            ClassicAssert.AreEqual(qHex, new BigInteger512(nativeRes).ToHexUnsigned(), "native.quotient");
            ClassicAssert.AreEqual(remHex, new BigInteger512(nativeRemainder).ToHexUnsigned(), "native.remainder");

            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned(), "quotient");
            ClassicAssert.AreEqual(remHex, remainder.ToHexUnsigned(), "remainder");
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