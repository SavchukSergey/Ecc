using System;
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

        [Test]
        public void DivRemTest() {
            var left = BigInteger256Ext.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256Ext.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");

            var res = BigInteger256.DivRem(left, right, out var reminder);
            ClassicAssert.AreEqual("0000000000000000000000000000000000000000000000000000000000001dd1", res.ToHexUnsigned());
            ClassicAssert.AreEqual("000281ef5cfc207bed7ea4cd27f4c9cb67ecbbe71173faa1d91045903b8a16c0", reminder.ToHexUnsigned());
        }

        [Test]
        public void ShiftLeftTest() {
            var bi = new BigInteger256(0x123456789abcdef);
            bi.AssignShiftLeft();
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            bi.TryWrite(buffer);
            ClassicAssert.AreEqual(0xde, buffer[0]);
            ClassicAssert.AreEqual(0x9b, buffer[1]);
            ClassicAssert.AreEqual(0x57, buffer[2]);
            ClassicAssert.AreEqual(0x13, buffer[3]);
            ClassicAssert.AreEqual(0xcf, buffer[4]);
            ClassicAssert.AreEqual(0x8a, buffer[5]);
            ClassicAssert.AreEqual(0x46, buffer[6]);
            ClassicAssert.AreEqual(0x02, buffer[7]);
            ClassicAssert.AreEqual(0x00, buffer[8]);
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

        [Test]
        public void ModAddTest() {
            ClassicAssert.AreEqual(
                new BigInteger256(5),
                new BigInteger256(6).ModAdd(new BigInteger256(126), new BigInteger256(127))
            );
        }

        [Test]
        public void MulTest() {
            var left = BigInteger256Ext.ParseHexUnsigned("0123456789abcdef");
            var right = BigInteger256Ext.ParseHexUnsigned("fedcba9876543210");
            var res = left * right;
            ClassicAssert.AreEqual("0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000121fa00ad77d7422236d88fe5618cf0", res.ToHexUnsigned());
        }
    }
}
