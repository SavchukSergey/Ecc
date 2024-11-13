using Ecc.Math;
using NUnit.Framework;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECProjectiveMontgomeryPoint256))]
    public class ECProjectiveMontgomeryPoint256Tests {

        [Test]
        public void ToAffinePointTest() {
            var curve = ECCurve256.Secp256k1;
            var montPoint = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("96e6b51d86cc74119556e2dad4965d5f385b8f72ab12613a2d7f3bcf74a81385"),
                BigInteger256.ParseHexUnsigned("6bc8994d2ff8b18e145821876517018ba9b7c533b221e9dfb639e5bfd55abdc4"),
                BigInteger256.ParseHexUnsigned("6e2c90fc3b29f9af00159d56fcfb4f80f2fea7c4fb96f0a5966664a8931caa3c"),
                curve
            );
            var affinePoint = montPoint.ToAffinePoint();
            AssertExt.AssertEquals(new ECPoint256(
                BigInteger256.ParseHexUnsigned("262331267d5dc527a9b4225337671f75228e97ea7137ea78a3fa7314bd1cedae"),
                BigInteger256.ParseHexUnsigned("cd35a5325fb802543006d5891056c9bde2b3683a58c945ec80d74b180baf78ee"),
                curve
            ), affinePoint);
        }

        [Test]
        public void ToProjectivePointTest() {
            var curve = ECCurve256.Secp256k1;
            var montPoint = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("96e6b51d86cc74119556e2dad4965d5f385b8f72ab12613a2d7f3bcf74a81385"),
                BigInteger256.ParseHexUnsigned("6bc8994d2ff8b18e145821876517018ba9b7c533b221e9dfb639e5bfd55abdc4"),
                BigInteger256.ParseHexUnsigned("6e2c90fc3b29f9af00159d56fcfb4f80f2fea7c4fb96f0a5966664a8931caa3c"),
                curve
            );
            var projectPoint = montPoint.ToProjective();
            AssertExt.AssertEquals(new ECProjectivePoint256(
                BigInteger256.ParseHexUnsigned("3ce6ec61299a964dbfd4d5587a14ac30ebb136abb909ea157c3c053acf88582c"),
                BigInteger256.ParseHexUnsigned("c4c3a8137d1426ecd61023b120bfea8968a8eb233d0e63b7ae3759df10383b82"),
                BigInteger256.ParseHexUnsigned("b4e2d4ab187ef003beb7fb6423ef3429d9153bcb78f5743c5ad6564e35842a1f"),
                curve
            ), projectPoint);
        }

        [Test]
        public void DoubleTest() {
            var curve = ECCurve256.Secp256k1;
            var source = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("96e6b51d86cc74119556e2dad4965d5f385b8f72ab12613a2d7f3bcf74a81385"),
                BigInteger256.ParseHexUnsigned("6bc8994d2ff8b18e145821876517018ba9b7c533b221e9dfb639e5bfd55abdc4"),
                BigInteger256.ParseHexUnsigned("6e2c90fc3b29f9af00159d56fcfb4f80f2fea7c4fb96f0a5966664a8931caa3c"),
                curve
            );
            var result = source.Double();
            var expected = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("0a84e433b730c6c2ab51c57861d3cc139b407e2f1e731d307e374b81e3e77cda"),
                BigInteger256.ParseHexUnsigned("441431e24f66fa8af308d4714505b1a0883295db56683c5fb9c8c42ed8080ad1"),
                BigInteger256.ParseHexUnsigned("5bbd52d7b5b0c5a0a05cb84d4253dc0cc1cde790951ee186504e327ef4c674ce"),
                curve
            );
            AssertExt.AssertEquals(expected, result);

            //recheck
            AssertExt.AssertEquals(expected.ToAffinePoint(), source.ToAffinePoint().Double());
        }
    }
}
