using Ecc.Math;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Numerics;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECPoint256))]
    public class ECPoint256Tests {

        [Test]
        public void AddTest() {
            var curve = new ECCurve256(name: null, a: -1, b: 3, modulus: new BigInteger256(127), order: default, cofactor: default, gx: default, gy: default);
            var p = curve.CreatePoint(new BigInteger256(16), new BigInteger256(20));
            var q = curve.CreatePoint(new BigInteger256(41), new BigInteger256(120));
            var r = p + q;
            ClassicAssert.AreEqual(new BigInteger(86), r.X);
            ClassicAssert.AreEqual(new BigInteger(81), r.Y);
        }

        [TestCase(16, 20, 97, 81)]
        [TestCase(41, 120, 42, 95)]
        public void AddSameTest(int sx, int sy, int tx, int ty) {
            var curve = new ECCurve256(name: null, a: -1, b: 3, modulus: new BigInteger256(127), order: default, cofactor: default, gx: default, gy: default);
            var p = curve.CreatePoint(new BigInteger256((uint)sx), new BigInteger256((uint)sy));
            var r = p + p;
            ClassicAssert.AreEqual(new BigInteger(tx), r.X);
            ClassicAssert.AreEqual(new BigInteger(ty), r.Y);
        }

        [TestCase(1, 3, 6)]
        [TestCase(2, 80, 10)]
        [TestCase(3, 80, 87)]
        [TestCase(4, 3, 91)]
        [TestCase(5, int.MaxValue, int.MaxValue)]
        [TestCase(6, 3, 6)]
        public void MulTest(int mul, int x, int y) {
            var curve = new ECCurve256(name: null, a: 2, b: 3, modulus: new BigInteger256(97), order: default, cofactor: default, gx: default, gy: default);
            var p1 = curve.CreatePoint(new BigInteger256(3), new BigInteger256(6));
            var p2 = p1 * new BigInteger256(mul);
            if (p2.IsInfinity) {
                ClassicAssert.IsTrue(x == int.MaxValue && y == int.MaxValue);
            } else {
                ClassicAssert.AreEqual(new BigInteger(x), p2.X);
                ClassicAssert.AreEqual(new BigInteger(y), p2.Y);
            }
        }

        [Test]
        public void GetHexTest() {
            ClassicAssert.AreEqual("0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798", ECCurve256.Secp256k1.G.GetHex());
            ClassicAssert.AreEqual("0479be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8", ECCurve.Secp256k1.G.GetHex(false));
        }

        [Test]
        public void InfinityGetHexTest() {
            ClassicAssert.AreEqual("00", ECPoint256.Infinity.GetHex());
        }

    }
}
