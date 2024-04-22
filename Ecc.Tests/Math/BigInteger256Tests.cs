using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Ecc.Math;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ecc.Tests.Math {
    [TestFixture]
    [TestOf(typeof(BigInteger256))]
    public partial class BigInteger256Tests {

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
            var bi = BigInteger256.ParseHexUnsigned("7846e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
            ClassicAssert.AreEqual("3cfcc1a04fafa2ddb6ca6869bf272715", bi.Low.ToString("x"));
        }

        [Test]
        public void HighTest() {
            var bi = BigInteger256.ParseHexUnsigned("7846e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
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
        public void ShiftVsDoublePerformanceTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var cnt = 10000;

            var swShift = new Stopwatch();
            swShift.Start();
            for (var i = 0; i < cnt; i++) {
                new BigInteger256(left).AssignLeftShift(1);
            }
            swShift.Stop();

            var swDouble = new Stopwatch();
            swDouble.Start();
            for (var i = 0; i < cnt; i++) {
                new BigInteger256(left).AssignDouble();
            }
            swDouble.Stop();

            Console.WriteLine($"mega shifts per second: {(double)cnt / 1e6 / swShift.Elapsed.TotalSeconds}");
            Console.WriteLine($"mega double per second: {(double)cnt / 1e6 / swDouble.Elapsed.TotalSeconds}");
        }

        [Test]
        public void ModMulPerformanceTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");

            var cnt = 10000;

            //warm up
            for (var i = 0; i < cnt; i++) {
                var _ = left.ModMul(right, modulus);
            }

            var swBits = new Stopwatch();
            swBits.Start();
            for (var i = 0; i < cnt; i++) {
                var _ = left.ModMul(right, modulus);
            }
            swBits.Stop();

            var leftN = left.ToNative();
            var rightN = right.ToNative();
            var modulusN = modulus.ToNative();
            var swNative = new Stopwatch();
            swNative.Start();
            for (var i = 0; i < cnt; i++) {
                var _ = leftN.ModMul(rightN, modulusN);
            }
            swNative.Stop();

            Console.WriteLine($"mega mod-mul bits per second: {(double)cnt / 1e6 / swBits.Elapsed.TotalSeconds}");
            Console.WriteLine($"mega mod-mul native per second: {(double)cnt / 1e6 / swNative.Elapsed.TotalSeconds}");
        }

        [Test]
        public void ModPowPerformanceTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");

            var cnt = 1000;

            var ctx = new MontgomeryContext256(modulus);

            //warm up
            for (var i = 0; i < cnt; i++) {
                var _ = left.ModPow(right, modulus);
            }

            var swNative = new Stopwatch();
            var leftN = left.ToNative();
            var rightN = right.ToNative();
            var modulusN = modulus.ToNative();
            swNative.Start();
            for (var i = 0; i < cnt; i++) {
                var _ = BigInteger.ModPow(leftN, rightN, modulusN);
            }
            swNative.Stop();

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < cnt; i++) {
                var _ = left.ModPow(right, modulus);
            }
            sw.Stop();

            var swMontgomery = new Stopwatch();
            swMontgomery.Start();
            for (var i = 0; i < cnt; i++) {
                var _ = left.ModPow(right, ctx);
            }
            swMontgomery.Stop();

            Console.WriteLine($"mega mod-pow per second: {(double)cnt / 1e6 / sw.Elapsed.TotalSeconds}");
            Console.WriteLine($"mega mod-pow montgomery per second: {(double)cnt / 1e6 / swMontgomery.Elapsed.TotalSeconds}");
            Console.WriteLine($"mega mod-pow native per second: {(double)cnt / 1e6 / swNative.Elapsed.TotalSeconds}");
        }

        [Test]
        public void MulPerformanceTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");

            var cnt = 10000;

            //warm up
            for (var i = 0; i < cnt; i++) {
                var _ = left * right;
            }

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < cnt; i++) {
                var _ = left * right;
            }
            sw.Stop();

            var leftN = left.ToNative();
            var rightN = right.ToNative();
            var swNative = new Stopwatch();
            swNative.Start();
            for (var i = 0; i < cnt; i++) {
                var _ = leftN * rightN;
            }
            swNative.Stop();

            Console.WriteLine($"mega mul per second: {(double)cnt / 1e6 / sw.Elapsed.TotalSeconds}");
            Console.WriteLine($"mega mul native per second: {(double)cnt / 1e6 / swNative.Elapsed.TotalSeconds}");
        }

        [Test]
        public void MulLowPerformanceTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");

            var cnt = 10000;

            //warm up
            for (var i = 0; i < cnt; i++) {
                BigInteger256.MulLow(left, right);
            }

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < cnt; i++) {
                BigInteger256.MulLow(left, right);
            }
            sw.Stop();

            Console.WriteLine($"mega mul-low per second: {(double)cnt / 1e6 / sw.Elapsed.TotalSeconds}");
        }

        [Test]
        public void AddPerformanceTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");

            var cnt = 10000;

            //warm up
            for (var i = 0; i < cnt; i++) {
                new BigInteger256(left).AssignAdd(right);
            }

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < cnt; i++) {
                new BigInteger256(left).AssignAdd(right);
            }
            sw.Stop();

            Console.WriteLine($"mega adds per second: {(double)cnt / 1e6 / sw.Elapsed.TotalSeconds}");
        }

        [Test]
        public void IncrementPerformanceTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");

            var cnt = 10000;

            //warm up
            for (var i = 0; i < cnt; i++) {
                new BigInteger256(left).AssignIncrement();
            }

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < cnt; i++) {
                new BigInteger256(left).AssignIncrement();
            }
            sw.Stop();

            Console.WriteLine($"mega increments per second: {(double)cnt / 1e6 / sw.Elapsed.TotalSeconds}");
        }

        [Test]
        public void DecrementPerformanceTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");

            var cnt = 10000;

            //warm up
            for (var i = 0; i < cnt; i++) {
                new BigInteger256(left).AssignDecrement();
            }

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < cnt; i++) {
                new BigInteger256(left).AssignDecrement();
            }
            sw.Stop();

            Console.WriteLine($"mega decrements per second: {(double)cnt / 1e6 / sw.Elapsed.TotalSeconds}");
        }

        [Test]
        public void SubPerformanceTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");

            var cnt = 10000;

            //warm up
            for (var i = 0; i < cnt; i++) {
                new BigInteger256(left).AssignSub(right);
            }

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < cnt; i++) {
                new BigInteger256(left).AssignSub(right);
            }
            sw.Stop();

            Console.WriteLine($"mega subs per second: {(double)cnt / 1e6 / sw.Elapsed.TotalSeconds}");
        }

        [Test]
        public void ModSubPerformanceTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
            var modulus = BigInteger256.ParseHexUnsigned("0xfffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");

            var cnt = 10000;

            //warm up
            for (var i = 0; i < cnt; i++) {
                var _ = left.ModSub(right, modulus);
            }

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < cnt; i++) {
                var _ = left.ModSub(right, modulus);
            }
            sw.Stop();

            Console.WriteLine($"mega mod-subs per second: {(double)cnt / 1e6 / sw.Elapsed.TotalSeconds}");
        }

        [Test]
        public void DivPerformanceBigTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("0006e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
            Console.WriteLine("Divide by u256");
            DivPerformanceTest(left, right);
        }

        [Test]
        public void DivPerformanceSmallTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var right = BigInteger256.ParseHexUnsigned("00000000000000000000000000000000000000000000000000000069bf272715");
            Console.WriteLine("Divide by u64");
            DivPerformanceTest(left, right);
        }

        private static void DivPerformanceTest(in BigInteger256 left, in BigInteger256 right) {

            var cnt = 10000;

            var swBits = new Stopwatch();
            swBits.Start();
            for (var i = 0; i < cnt; i++) {
                BigInteger256.DivRemBits(left, right, out var _);
            }
            swBits.Stop();

            var swBitsFull = new Stopwatch();
            swBitsFull.Start();
            for (var i = 0; i < cnt; i++) {
                BigInteger256.DivRemFullBits(left, right, out var _);
            }
            swBitsFull.Stop();

            var ln = left.ToNative();
            var rn = right.ToNative();

            var swNative = new Stopwatch();
            swNative.Start();
            for (var i = 0; i < cnt; i++) {
                BigInteger.DivRem(ln, rn, out var _);
            }
            swNative.Stop();

            var sw3 = new Stopwatch();
            sw3.Start();
            for (var i = 0; i < cnt; i++) {
                BigInteger256.DivRemNewton(left, right, out var _);
            }
            sw3.Stop();

            var sw4 = new Stopwatch();
            sw4.Start();
            for (var i = 0; i < cnt; i++) {
                BigInteger256.DivRemGuess(left, right, out var _);
            }
            sw4.Stop();

            var format = (Stopwatch sw) => {
                var val = ((double)cnt / sw.Elapsed.TotalSeconds);
                val = System.Math.Round(val);
                return val.ToString().PadLeft(10, ' ');
            };

            Console.WriteLine($"Ecc div bits per second:      {format(swBits)}");
            Console.WriteLine($"Ecc div full bits per second: {format(swBitsFull)}");
            Console.WriteLine($"Ecc div newton per second:    {format(sw3)}");
            Console.WriteLine($"Ecc div guess per second:     {format(sw4)}");
            Console.WriteLine($"Native div per second:        {format(swNative)}");
        }

        [Test]
        public void AssignLeftShiftHalfTest() {
            var bi = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
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

        #region Modular

        #region Multiplication

        [Test]
        public void ModMulBitTest() {
            ClassicAssert.AreEqual(
              "f68d6baa084effcf7222c3f72d9ae49c974ced4078afe384291b7966149ac12c",
               BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5").ModMulBit(
                    BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5"),
                    BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f")
                ).ToHexUnsigned()
           );
        }

        [Test]
        public void ModMulZeroLeftTest() {
            ClassicAssert.AreEqual(
              "0000000000000000000000000000000000000000000000000000000000000000",
               new BigInteger256().ModMul(
                    BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5"),
                    BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f")
                ).ToHexUnsigned()
           );
        }

        [Test]
        public void ModMulZeroRightTest() {
            ClassicAssert.AreEqual(
              "0000000000000000000000000000000000000000000000000000000000000000",
               BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5").ModMul(
                    new BigInteger256(),
                    BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f")
                ).ToHexUnsigned()
           );
        }

        [Test]
        public void ModMulZeroTest() {
            ClassicAssert.AreEqual(
              "0000000000000000000000000000000000000000000000000000000000000000",
               new BigInteger256().ModMul(new BigInteger256(), BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f")).ToHexUnsigned()
           );
        }

        [Test]
        public void ModInverseTest() {
            var value = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");

            ClassicAssert.AreEqual(
              "6a6c59bdd7a25d5fcd3b69d7f8b183194451df6a625bbecf68e7d86e194c21ac",
               value.ModInverse(modulus).ToHexUnsigned()
           );
        }

        [Test]
        public void ModInversePerformanceTest() {
            var left = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var modulus = BigInteger256.ParseHexUnsigned("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f");

            var cnt = 1000;

            //warm up
            for (var i = 0; i < cnt; i++) {
                var _ = left.ModInverse(modulus);
            }

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < cnt; i++) {
                var _ = left.ModInverse(modulus);
            }
            sw.Stop();

            var swNative = new Stopwatch();
            var leftN = left.ToNative();
            var modulusN = modulus.ToNative();
            swNative.Start();
            for (var i = 0; i < cnt; i++) {
                var _ = leftN.ModInverse(modulusN);
            }
            swNative.Stop();

            Console.WriteLine($"mega modInverse per second: {(double)cnt / 1e6 / sw.Elapsed.TotalSeconds}");
            Console.WriteLine($"mega modInverse native per second: {(double)cnt / 1e6 / swNative.Elapsed.TotalSeconds}");
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
