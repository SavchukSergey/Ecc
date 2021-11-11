using System;
using Ecc.Math;
using NUnit.Framework;

namespace Ecc.Tests.Math {
    [TestFixture]
    [TestOf(typeof(BigInteger512))]
    public class BigInteger512Tests {

        [Test]
        public void CtorTest() {
            Span<byte> buffer = stackalloc byte[BigInteger512.BYTES_SIZE];
            var bi = new BigInteger512(0x65234512);
            bi.TryWrite(buffer);
            Assert.AreEqual(0x12, buffer[0]);
            Assert.AreEqual(0x45, buffer[1]);
            Assert.AreEqual(0x23, buffer[2]);
            Assert.AreEqual(0x65, buffer[3]);
        }

        [Test]
        public void CtorU32Test() {
            Span<byte> buffer = stackalloc byte[BigInteger512.BYTES_SIZE];
            var bi = new BigInteger512(0x0123456789abcdeful);
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
            var bi512 = new BigInteger512();
            bi512.Clear();
            Assert.IsTrue(bi512.IsZero);
        }

        [Test]
        public void AddTest() {
            var left = new BigInteger512(0x80000000);
            var right = new BigInteger512(0x84000001);
            left.Add(right);
            Span<byte> buffer = stackalloc byte[BigInteger512.BYTES_SIZE];
            left.TryWrite(buffer);
            Assert.AreEqual(0x01, buffer[0]);
            Assert.AreEqual(0x00, buffer[1]);
            Assert.AreEqual(0x00, buffer[2]);
            Assert.AreEqual(0x04, buffer[3]);
            Assert.AreEqual(0x01, buffer[4]);
        }

        [Test]
        public void CompareEqTest() {
            var left = new BigInteger512(0x0123456789abcdeful);
            var right = new BigInteger512(0x0123456789abcdeful);
            Assert.AreEqual(0, left.Compare(right));
        }

        [Test]
        public void CompareLessTest() {
            var left = new BigInteger512(0x0123456789abcdeful);
            var right = new BigInteger512(0x0123456789abcdfful);
            Assert.AreEqual(-1, left.Compare(right));
        }

        [Test]
        public void CompareGreaterTest() {
            var left = new BigInteger512(0xf123456789abcdeful);
            var right = new BigInteger512(0x0123456789abcdeful);
            Assert.AreEqual(1, left.Compare(right));
        }

    }
}
