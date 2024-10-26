using System;
using System.Collections.Generic;
using System.Numerics;
using Ecc.Math;
using NUnit.Framework;

namespace Ecc.Tests.Math {
    public partial class BigInteger256Tests {

        [Test]
        public void MulTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");

            var result = left * right;

            Assert.That(result.ToHexUnsigned(), Is.EqualTo("0005875c7f8293233d1997ee85937f86e630f16bbc37b05cd468c24fa0981e89c8e132223ea0efaaacc367e707f312184698acd764db94aa6dc65ba8080cc0c9"));
        }

        [Test]
        public void SquareTest() {
            Assert.That(
                BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5").Square().ToHexUnsigned(),
                Is.EqualTo("a4db0018f81780ee1ecebfedb2677795e8132dee14be8f58e853e8ea853dda55d6aa8b7117cd331e2cb0d23b68a85393d15c144a651ae84a56c6b3076bb7c2d9")
            );
        }

        [Test]
        public void MulHighTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");

            var result = BigInteger256.MulHigh(left, right);

            Assert.That(result.ToHexUnsigned(), Is.EqualTo("0005875c7f8293233d1997ee85937f86e630f16bbc37b05cd468c24fa0981e89"));
        }

        [Test]
        public void MulLow256Test() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");

            var result = BigInteger256.MulLow256(left, right);

            Assert.That(result.ToHexUnsigned(), Is.EqualTo("c8e132223ea0efaaacc367e707f312184698acd764db94aa6dc65ba8080cc0c9"));
        }

        [Test]
        public void ModMulTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");

            var result = left.ModMul(right, modulus);

            Assert.That(result.ToHexUnsigned(), Is.EqualTo("5d7d5d481d0e11156103bcb9b60db5588399b95d89cc6940b231937201b2e38f"));
        }

        [Test]
        public void ModSquareTest() {
            var result = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5").ModSquare(
                BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f")
            );
            Assert.That(
                result.ToHexUnsigned(),
                Is.EqualTo("f68d6baa084effcf7222c3f72d9ae49c974ced4078afe384291b7966149ac12c")
            );
        }

        [TestCaseSource(nameof(ModPowCases))]
        public void ModPowTest(string valueHex, string expHex, string expectedHex) {
            var value = BigInteger256.ParseHexUnsigned(valueHex);
            var exponent = BigInteger256.ParseHexUnsigned(expHex);
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");

            var result = value.ModPow(exponent, modulus);

            Assert.That(result.ToHexUnsigned(), Is.EqualTo(expectedHex));
        }

        [TestCaseSource(nameof(ModPowCases))]
        public void ModPowMontgomeryTest(string valueHex, string expHex, string expectedHex) {
            var value = BigInteger256.ParseHexUnsigned(valueHex);
            var exponent = BigInteger256.ParseHexUnsigned(expHex);
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");

            var ctx = new MontgomeryContext256(modulus);

            var result = value.ModPow(exponent, ctx);

            Assert.That(result.ToHexUnsigned(), Is.EqualTo(expectedHex));
        }

        [Test]
        public void GenerateModMulMontgomeryTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5").ToNative();
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715").ToNative();
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f").ToNative();
            Console.WriteLine($"left:      {new BigInteger256(left).ToHexUnsigned()}");
            Console.WriteLine($"right:     {new BigInteger256(right).ToHexUnsigned()}");
            Console.WriteLine($"modulus:   {new BigInteger256(modulus).ToHexUnsigned()}");

            var shift = 511;
            var baseK = new BigInteger(1) << shift;
            var gcd = BigInteger.GreatestCommonDivisor(modulus, baseK);
            Console.WriteLine($"gcd: {gcd.ToString("x").TrimStart('0')}");

            var reciprocalModulus = (baseK + modulus - 1) / modulus;
            Console.WriteLine($"rec. mod.: {reciprocalModulus.ToString("x").TrimStart('0')}");
            Console.WriteLine($"mod check: {(reciprocalModulus * modulus).ToString("x").TrimStart('0')}");
            var mul = (left * right);
            Console.WriteLine($"mul:       {mul.ToString("x").TrimStart('0')}");
            var mod = mul % modulus;
            Console.WriteLine($"mod:       {mod.ToString("x").TrimStart('0')}");
            var q = mul / modulus;
            Console.WriteLine($"q:         {q.ToString("x").TrimStart('0')}");

            var resultRaw = (mul * reciprocalModulus);
            Console.WriteLine($"resultRaw: {resultRaw.ToString("x").TrimStart('0')}");

            var result = resultRaw >> shift;
            Console.WriteLine($"result:    {result.ToString("x").TrimStart('0')}");

        }


        [Test]
        public void GenerateBarrettReductionConstTest() {
            var modulusN = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f").ToNative();
            var k = 256;
            var d = new BigInteger(1) << (k * 2);
            var cN = d / modulusN;
            var c = new BigInteger512(cN);
            Console.WriteLine($"result:    {c.ToHexUnsigned()}");
        }

        [Test]
        public void ModMulMontgonmeryAlgoTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");

            var ctx = new MontgomeryContext256(modulus);

            Assert.That(ctx.IsValid, Is.True);

            var um = ctx.ToMontgomery(left);
            var vm = ctx.ToMontgomery(right);

            var uvr = ctx.ModMul(um, vm);
            var result = ctx.Reduce(uvr);

            Assert.That(result.ToHexUnsigned(), Is.EqualTo("5d7d5d481d0e11156103bcb9b60db5588399b95d89cc6940b231937201b2e38f"));
        }

        public static IEnumerable<string[]> ModPowCases() {
            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715",
                "01407be969151e9402e5cdd6462676d38085ed57927ad147ea89f88055741b01",
            ];

            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000000000000000000000000000000000000000000000000000000000000",
                "0000000000000000000000000000000000000000000000000000000000000001",
            ];

            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000000000000000000000000000000000000000000000000000000000001",
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
            ];

            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000000000000000000000000000000000000000000000000000000000002",
                "f68d6baa084effcf7222c3f72d9ae49c974ced4078afe384291b7966149ac12c",
            ];

            yield return [
                "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5",
                "0000000000000000000000000000000000000000000000000000000000000004",
                "4b6401fe5e473e64611d2b20187f103af350633cf0d4d487fe5435683223210d",
            ];

        }

    }
}