using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;
using Ecc.Math;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ecc.Tests.Math {
    [TestFixture]
    [TestOf(typeof(BigInteger256))]
    public class BigInteger256Tests {

        [Test]
        public void CtorTest() {
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            var bi = new BigInteger256(0x65234511);
            bi.TryWrite(buffer);
            ClassicAssert.AreEqual(0x11, buffer[0]);
            ClassicAssert.AreEqual(0x45, buffer[1]);
            ClassicAssert.AreEqual(0x23, buffer[2]);
            ClassicAssert.AreEqual(0x65, buffer[3]);
        }

        [Test]
        public void CtorU32Test() {
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            var bi = new BigInteger256(0x0123456789abcdeful);
            bi.TryWrite(buffer);
            ClassicAssert.AreEqual(0xef, buffer[0]);
            ClassicAssert.AreEqual(0xcd, buffer[1]);
            ClassicAssert.AreEqual(0xab, buffer[2]);
            ClassicAssert.AreEqual(0x89, buffer[3]);
            ClassicAssert.AreEqual(0x67, buffer[4]);
            ClassicAssert.AreEqual(0x45, buffer[5]);
            ClassicAssert.AreEqual(0x23, buffer[6]);
            ClassicAssert.AreEqual(0x01, buffer[7]);
            ClassicAssert.AreEqual(0x00, buffer[8]);
        }

        [Test]
        public void CtorU128Test() {
            var lower = new UInt128(0xfedcba9876543210, 0x0123456789abcdeful);
            var bi = new BigInteger256(lower);
            ClassicAssert.AreEqual("00000000000000000000000000000000fedcba98765432100123456789abcdef", bi.ToHexUnsigned());
        }


        [Test]
        public void LowTest() {
            var bi = BigInteger256Ext.ParseHexUnsigned("7846e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
            ClassicAssert.AreEqual("3cfcc1a04fafa2ddb6ca6869bf272715", bi.Low.ToString("x"));
        }

        [Test]
        public void HighTest() {
            var bi = BigInteger256Ext.ParseHexUnsigned("7846e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
            ClassicAssert.AreEqual("7846e3be8abd2e089ed812475be9b51c", bi.High.ToString("x"));
        }


        [Test]
        public void IsZeroTest() {
            ClassicAssert.IsTrue(new BigInteger256(0).IsZero);
            ClassicAssert.IsFalse(new BigInteger256(1).IsZero);
            ClassicAssert.IsFalse(new BigInteger256(1, 2, 3, 4).IsZero);
        }

        [Test]
        public void ReadFromHexTest() {
            var bi = new BigInteger256();
            bi.ReadFromHex("0123456789abcdef");
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            bi.TryWrite(buffer);
            ClassicAssert.AreEqual(0xef, buffer[0]);
            ClassicAssert.AreEqual(0xcd, buffer[1]);
            ClassicAssert.AreEqual(0xab, buffer[2]);
            ClassicAssert.AreEqual(0x89, buffer[3]);
            ClassicAssert.AreEqual(0x67, buffer[4]);
            ClassicAssert.AreEqual(0x45, buffer[5]);
            ClassicAssert.AreEqual(0x23, buffer[6]);
            ClassicAssert.AreEqual(0x01, buffer[7]);
            ClassicAssert.AreEqual(0x00, buffer[8]);
        }

        [Test]
        public void ReadFromHexHalfByteTest() {
            var bi = new BigInteger256();
            bi.ReadFromHex("f");
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            bi.TryWrite(buffer);
            ClassicAssert.AreEqual(0x0f, buffer[0]);
            ClassicAssert.AreEqual(0x00, buffer[1]);
        }

        [Test]
        public void AddTest() {
            var left = new BigInteger256(0x80000000);
            var right = new BigInteger256(0x84000001);
            left.AssignAdd(right);
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            left.TryWrite(buffer);
            ClassicAssert.AreEqual(0x01, buffer[0]);
            ClassicAssert.AreEqual(0x00, buffer[1]);
            ClassicAssert.AreEqual(0x00, buffer[2]);
            ClassicAssert.AreEqual(0x04, buffer[3]);
            ClassicAssert.AreEqual(0x01, buffer[4]);
        }

        [Test]
        public void SubPosTest() {
            var left = new BigInteger256(0x284000001);
            var right = new BigInteger256(0x180000000);
            left.AssignSub(right);
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            left.TryWrite(buffer);
            ClassicAssert.AreEqual(0x01, buffer[0]);
            ClassicAssert.AreEqual(0x00, buffer[1]);
            ClassicAssert.AreEqual(0x00, buffer[2]);
            ClassicAssert.AreEqual(0x04, buffer[3]);
            ClassicAssert.AreEqual(0x01, buffer[4]);
            ClassicAssert.AreEqual(0x00, buffer[5]);
        }

        [Test]
        public void SubNegTest() {
            var left = new BigInteger256(0x180000000);
            var right = new BigInteger256(0x284000001);
            left.AssignSub(right);
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            left.TryWrite(buffer);
            ClassicAssert.AreEqual(0xff, buffer[0]);
            ClassicAssert.AreEqual(0xff, buffer[1]);
            ClassicAssert.AreEqual(0xff, buffer[2]);
            ClassicAssert.AreEqual(0xfb, buffer[3]);
            ClassicAssert.AreEqual(0xfe, buffer[4]);
            ClassicAssert.AreEqual(0xff, buffer[5]);
        }

        [TestCaseSource(nameof(DivideCases))]
        public void DivRemNativeTest(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger256Ext.ParseHexUnsigned(leftHex);
            var right = BigInteger256Ext.ParseHexUnsigned(rightHex);

            var res = BigInteger.DivRem(left.ToNative(), right.ToNative(), out var reminder);
            ClassicAssert.AreEqual(qHex, new BigInteger256(res).ToHexUnsigned());
            ClassicAssert.AreEqual(remHex, new BigInteger256(reminder).ToHexUnsigned());
        }

        [TestCaseSource(nameof(DivideCases))]
        public void DivRemTest(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger256Ext.ParseHexUnsigned(leftHex);
            var right = BigInteger256Ext.ParseHexUnsigned(rightHex);

            var res = BigInteger256.DivRem(left, right, out var reminder);
            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned());
            ClassicAssert.AreEqual(remHex, reminder.ToHexUnsigned());
        }

        [TestCaseSource(nameof(DivideCases))]
        public void DivRemBitsTest(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger256Ext.ParseHexUnsigned(leftHex);
            var right = BigInteger256Ext.ParseHexUnsigned(rightHex);

            var res = BigInteger256.DivRemBits(left, right, out var reminder);
            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned());
            ClassicAssert.AreEqual(remHex, reminder.ToHexUnsigned());
        }

        [TestCaseSource(nameof(DivideCases))]
        public void DivRemNewtonTest(string leftHex, string rightHex, string qHex, string remHex) {
            var left = BigInteger256Ext.ParseHexUnsigned(leftHex);
            var right = BigInteger256Ext.ParseHexUnsigned(rightHex);

            var res = BigInteger256.DivRemNewton(left, right, out var reminder);
            ClassicAssert.AreEqual(qHex, res.ToHexUnsigned());
            ClassicAssert.AreEqual(remHex, reminder.ToHexUnsigned());
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
        }

        [Test]
        public void DivPerformanceTest() {
            var left = BigInteger256Ext.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256Ext.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");

            var cnt = 10000;

            var sw1 = new Stopwatch();
            sw1.Start();
            for (var i = 0; i < cnt; i++) {
                //todo: improve lib division
                BigInteger256.DivRem(left, right, out var _);
            }
            sw1.Stop();

            var ln = left.ToNative();
            var rn = right.ToNative();

            var sw2 = new Stopwatch();
            sw2.Start();
            for (var i = 0; i < cnt; i++) {
                BigInteger.DivRem(ln, rn, out var _);
            }
            sw2.Stop();

            var sw3 = new Stopwatch();
            sw3.Start();
            for (var i = 0; i < cnt; i++) {
                BigInteger256.DivRemNewton(left, right, out var _);
            }
            sw3.Stop();

            Console.WriteLine($"Ecc div per second: {(double)cnt / sw1.Elapsed.TotalSeconds}");
            Console.WriteLine($"Ecc div newton per second: {(double)cnt / sw3.Elapsed.TotalSeconds}");
            Console.WriteLine($"Native div per second: {(double)cnt / sw2.Elapsed.TotalSeconds}");
        }

        [Test]
        public void AssignLeftShiftHalfTest() {
            var bi = BigInteger256Ext.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            bi.AssignLeftShiftHalf();
            ClassicAssert.AreEqual("d435f8cf054b0f5902237e8cb9ee5fe500000000000000000000000000000000", bi.ToHexUnsigned());
        }

        [Test]
        public void CompareEqTest() {
            var left = new BigInteger256(0x0123456789abcdeful);
            var right = new BigInteger256(0x0123456789abcdeful);
            ClassicAssert.AreEqual(0, left.Compare(right));
        }

        [Test]
        public void CompareLessTest() {
            var left = new BigInteger256(0x0123456789abcdeful);
            var right = new BigInteger256(0x0123456789abcdfful);
            ClassicAssert.AreEqual(-1, left.Compare(right));
        }

        [Test]
        public void CompareGreaterTest() {
            var left = new BigInteger256(0xf123456789abcdeful);
            var right = new BigInteger256(0x0123456789abcdeful);
            ClassicAssert.AreEqual(1, left.Compare(right));
        }


        [Test]
        public void ModDoubleTest() {
            ClassicAssert.AreEqual(new BigInteger256(40), new BigInteger256(20).ModDouble(new BigInteger256(127)));
            ClassicAssert.AreEqual(new BigInteger256(80), new BigInteger256(40).ModDouble(new BigInteger256(127)));
            ClassicAssert.AreEqual(new BigInteger256(33), new BigInteger256(80).ModDouble(new BigInteger256(127)));
        }

        #region Multiplification

        [Test]
        public void MulTest() {
            ClassicAssert.AreEqual(
              "a4db0018f81780ee1ecebfedb2677795e8132dee14be8f58e853e8ea853dda55d6aa8b7117cd331e2cb0d23b68a85393d15c144a651ae84a56c6b3076bb7c2d9",
                (
                    BigInteger256Ext.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5") *
                    BigInteger256Ext.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5")
                ).ToHexUnsigned()
           );
        }

        #endregion

        #region Modular

        #region Multiplication

        [Test]
        public void ModMulTest() {
            ClassicAssert.AreEqual(
              "f68d6baa084effcf7222c3f72d9ae49c974ced4078afe384291b7966149ac12c",
               BigInteger256Ext.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5").ModMul(
                    BigInteger256Ext.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5"),
                    BigInteger256Ext.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f")
                ).ToHexUnsigned()
           );
        }

        [Test]
        public void ModMulBitTest() {
            ClassicAssert.AreEqual(
              "f68d6baa084effcf7222c3f72d9ae49c974ced4078afe384291b7966149ac12c",
               BigInteger256Ext.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5").ModMulBit(
                    BigInteger256Ext.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5"),
                    BigInteger256Ext.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f")
                ).ToHexUnsigned()
           );
        }

        [Test]
        public void ModMulZeroLeftTest() {
            ClassicAssert.AreEqual(
              "0000000000000000000000000000000000000000000000000000000000000000",
               new BigInteger256().ModMul(
                    BigInteger256Ext.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5"),
                    BigInteger256Ext.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f")
                ).ToHexUnsigned()
           );
        }

        [Test]
        public void ModMulZeroRightTest() {
            ClassicAssert.AreEqual(
              "0000000000000000000000000000000000000000000000000000000000000000",
               BigInteger256Ext.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5").ModMul(
                    new BigInteger256(),
                    BigInteger256Ext.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f")
                ).ToHexUnsigned()
           );
        }

        [Test]
        public void ModMulZeroTest() {
            ClassicAssert.AreEqual(
              "0000000000000000000000000000000000000000000000000000000000000000",
               new BigInteger256().ModMul(new BigInteger256(), BigInteger256Ext.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f")).ToHexUnsigned()
           );
        }

        [Test]
        public void ModInverseTest() {
            ClassicAssert.AreEqual(
              "6a6c59bdd7a25d5fcd3b69d7f8b183194451df6a625bbecf68e7d86e194c21ac",
               BigInteger256Ext.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5").ModInverse(
                    BigInteger256Ext.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f")
                ).ToHexUnsigned()
           );
        }

        #endregion

        #endregion

        [Test]
        public void ModAddTest() {
            ClassicAssert.AreEqual(
                new BigInteger256(5),
                new BigInteger256(6).ModAdd(new BigInteger256(126), new BigInteger256(127))
            );
        }
    }
}
