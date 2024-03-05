using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Legacy;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(BigRefInteger))]
    public class BigRefntegerExtTests {

        [Test]
        public void ParseHexUnsignedTest() {
            var val = new BigRefInteger {
                Data = stackalloc byte[12]
            };
            BigRefInteger.ParseHexUnsigned("a04529b5ef8c", ref val);
            ClassicAssert.AreEqual(0x8c, val.Data[0]);
            ClassicAssert.AreEqual(0xef, val.Data[1]);
            ClassicAssert.AreEqual(0xb5, val.Data[2]);
            ClassicAssert.AreEqual(0x29, val.Data[3]);
            ClassicAssert.AreEqual(0x45, val.Data[4]);
            ClassicAssert.AreEqual(0xa0, val.Data[5]);
        }

        [Test]
        public void ToBigEndianBytesTest() {
            var val = new BigRefInteger {
                Data = [0xa0, 0x45, 0x29, 0xb5, 0xef, 0x8c]
            };
            Span<byte> res = stackalloc byte[val.Data.Length];
            val.ToBigEndianBytes(res);

            ClassicAssert.AreEqual(0x8c, res[0]);
            ClassicAssert.AreEqual(0xef, res[1]);
            ClassicAssert.AreEqual(0xb5, res[2]);
            ClassicAssert.AreEqual(0x29, res[3]);
            ClassicAssert.AreEqual(0x45, res[4]);
            ClassicAssert.AreEqual(0xa0, res[5]);
        }

    }
}
