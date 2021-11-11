using System;
using Ecc.Math;
using NUnit.Framework;

namespace Ecc.Tests.Math {
    [TestFixture]
    [TestOf(typeof(BigInteger256))]
    public class BigInteger256Tests {

        [Test]
        public void CtorTest() {
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            var bi = new BigInteger256(0x65234511);
            bi.TryWrite(buffer);
            Assert.AreEqual(0x11, buffer[0]);
            Assert.AreEqual(0x45, buffer[1]);
            Assert.AreEqual(0x23, buffer[2]);
            Assert.AreEqual(0x65, buffer[3]);
        }

        [Test]
        public void CtorU32Test() {
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            var bi = new BigInteger256(0x0123456789abcdeful);
            bi.TryWrite(buffer);
            Assert.AreEqual(0xef, buffer[0]);
            Assert.AreEqual(0xcd, buffer[1]);
            Assert.AreEqual(0xab, buffer[2]);
            Assert.AreEqual(0x89, buffer[3]);
            Assert.AreEqual(0x67, buffer[4]);
            Assert.AreEqual(0x45, buffer[5]);
            Assert.AreEqual(0x23, buffer[6]);
            Assert.AreEqual(0x01, buffer[7]);
            Assert.AreEqual(0x00, buffer[8]);
        }

        [Test]
        public void ClearIsZeroTest() {
            var bi = new BigInteger256();
            bi.Clear();
            Assert.IsTrue(bi.IsZero);
        }

        [Test]
        public void ReadFromHexTest() {
            var bi = new BigInteger256();
            bi.ReadFromHex("0123456789abcdef");
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            bi.TryWrite(buffer);
            Assert.AreEqual(0xef, buffer[0]);
            Assert.AreEqual(0xcd, buffer[1]);
            Assert.AreEqual(0xab, buffer[2]);
            Assert.AreEqual(0x89, buffer[3]);
            Assert.AreEqual(0x67, buffer[4]);
            Assert.AreEqual(0x45, buffer[5]);
            Assert.AreEqual(0x23, buffer[6]);
            Assert.AreEqual(0x01, buffer[7]);
            Assert.AreEqual(0x00, buffer[8]);
        }

        [Test]
        public void ReadFromHexHalfByteTest() {
            var bi = new BigInteger256();
            bi.ReadFromHex("f");
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            bi.TryWrite(buffer);
            Assert.AreEqual(0x0f, buffer[0]);
            Assert.AreEqual(0x00, buffer[1]);
        }

        [Test]
        public void AddTest() {
            var left = new BigInteger256(0x80000000);
            var right = new BigInteger256(0x84000001);
            left.Add(right);
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            left.TryWrite(buffer);
            Assert.AreEqual(0x01, buffer[0]);
            Assert.AreEqual(0x00, buffer[1]);
            Assert.AreEqual(0x00, buffer[2]);
            Assert.AreEqual(0x04, buffer[3]);
            Assert.AreEqual(0x01, buffer[4]);
        }

        [Test]
        public void SubPosTest() {
            var left = new BigInteger256(0x284000001);
            var right = new BigInteger256(0x180000000);
            left.Sub(right);
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            left.TryWrite(buffer);
            Assert.AreEqual(0x01, buffer[0]);
            Assert.AreEqual(0x00, buffer[1]);
            Assert.AreEqual(0x00, buffer[2]);
            Assert.AreEqual(0x04, buffer[3]);
            Assert.AreEqual(0x01, buffer[4]);
            Assert.AreEqual(0x00, buffer[5]);
        }

        [Test]
        public void SubNegTest() {
            var left = new BigInteger256(0x180000000);
            var right = new BigInteger256(0x284000001);
            left.Sub(right);
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            left.TryWrite(buffer);
            Assert.AreEqual(0xff, buffer[0]);
            Assert.AreEqual(0xff, buffer[1]);
            Assert.AreEqual(0xff, buffer[2]);
            Assert.AreEqual(0xfb, buffer[3]);
            Assert.AreEqual(0xfe, buffer[4]);
            Assert.AreEqual(0xff, buffer[5]);
        }

        [Test]
        public void ShiftLeftTest() {
            var bi = new BigInteger256(0x123456789abcdef);
            bi.ShiftLeft();
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            bi.TryWrite(buffer);
            Assert.AreEqual(0xde, buffer[0]);
            Assert.AreEqual(0x9b, buffer[1]);
            Assert.AreEqual(0x57, buffer[2]);
            Assert.AreEqual(0x13, buffer[3]);
            Assert.AreEqual(0xcf, buffer[4]);
            Assert.AreEqual(0x8a, buffer[5]);
            Assert.AreEqual(0x46, buffer[6]);
            Assert.AreEqual(0x02, buffer[7]);
            Assert.AreEqual(0x00, buffer[8]);
        }

        [Test]
        public void ZeroExtend128Test() {
            var source = new BigInteger128(0x123456789abcdef);
            var bi = new BigInteger256(source);
            Span<byte> buffer = stackalloc byte[BigInteger256.BYTES_SIZE];
            bi.TryWrite(buffer);
            Assert.AreEqual(0xef, buffer[0]);
            Assert.AreEqual(0xcd, buffer[1]);
            Assert.AreEqual(0xab, buffer[2]);
            Assert.AreEqual(0x89, buffer[3]);
            Assert.AreEqual(0x67, buffer[4]);
            Assert.AreEqual(0x45, buffer[5]);
            Assert.AreEqual(0x23, buffer[6]);
            Assert.AreEqual(0x01, buffer[7]);
            Assert.AreEqual(0x00, buffer[8]);
        }

        [Test]
        public void CompareEqTest() {
            var left = new BigInteger256(0x0123456789abcdeful);
            var right = new BigInteger256(0x0123456789abcdeful);
            Assert.AreEqual(0, left.Compare(right));
        }

        [Test]
        public void CompareLessTest() {
            var left = new BigInteger256(0x0123456789abcdeful);
            var right = new BigInteger256(0x0123456789abcdfful);
            Assert.AreEqual(-1, left.Compare(right));
        }

        [Test]
        public void CompareGreaterTest() {
            var left = new BigInteger256(0xf123456789abcdeful);
            var right = new BigInteger256(0x0123456789abcdeful);
            Assert.AreEqual(1, left.Compare(right));
        }

    }
}
