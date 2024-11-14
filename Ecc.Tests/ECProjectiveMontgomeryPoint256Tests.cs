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
        public void NegateTest() {
            var curve = ECCurve256.Secp256k1;
            var source = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("96e6b51d86cc74119556e2dad4965d5f385b8f72ab12613a2d7f3bcf74a81385"),
                BigInteger256.ParseHexUnsigned("6bc8994d2ff8b18e145821876517018ba9b7c533b221e9dfb639e5bfd55abdc4"),
                BigInteger256.ParseHexUnsigned("6e2c90fc3b29f9af00159d56fcfb4f80f2fea7c4fb96f0a5966664a8931caa3c"),
                curve
            );
            var result = source.Negate();
            var expected = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("96e6b51d86cc74119556e2dad4965d5f385b8f72ab12613a2d7f3bcf74a81385"),
                BigInteger256.ParseHexUnsigned("943766b2d0074e71eba7de789ae8fe7456483acc4dde162049c61a3f2aa53e6b"),
                BigInteger256.ParseHexUnsigned("6e2c90fc3b29f9af00159d56fcfb4f80f2fea7c4fb96f0a5966664a8931caa3c"),
                curve
            );
            AssertExt.AssertEquals(expected, result);

            //recheck
            AssertExt.AssertEquals(expected.ToAffinePoint(), source.ToAffinePoint().Negate());
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

        [Test]
        public void AddTest() {
            var curve = ECCurve256.Secp256k1;
            var left = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("96e6b51d86cc74119556e2dad4965d5f385b8f72ab12613a2d7f3bcf74a81385"),
                BigInteger256.ParseHexUnsigned("6bc8994d2ff8b18e145821876517018ba9b7c533b221e9dfb639e5bfd55abdc4"),
                BigInteger256.ParseHexUnsigned("6e2c90fc3b29f9af00159d56fcfb4f80f2fea7c4fb96f0a5966664a8931caa3c"),
                curve
            );
            var right = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("d0a9287039b5b1d143e6f6ecf21936dcfb8994292b94fce0f1730f963e82f378"),
                BigInteger256.ParseHexUnsigned("a4f4129df588b6c08ffa58d172c136b52d6f855ee3b15a3e9392fe70d08ea32d"),
                BigInteger256.ParseHexUnsigned("833579bd538e7d654116041a99b4dba37ad1aaa4bef5967137a146afde8e8463"),
                curve
            );
            var result = left + right;
            var expected = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("a3cf2eab4d1af2fa5361ea6866e2038dc33ad52739764c20a51ae7fdff068b29"),
                BigInteger256.ParseHexUnsigned("558d35135f8890aabb298ab454c7cc301bd8370169339d93f655a1157c4d32a3"),
                BigInteger256.ParseHexUnsigned("a9ee70f267e39c0a0309190a6874754949ee2388913b440f27d5fe348b755e9a"),
                curve
            );
            AssertExt.AssertEquals(expected, result);

            //recheck
            AssertExt.AssertEquals(expected.ToAffinePoint(), (left + right).ToAffinePoint());
        }

        [Test]
        public void SubTest() {
            var curve = ECCurve256.Secp256k1;
            var left = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("96e6b51d86cc74119556e2dad4965d5f385b8f72ab12613a2d7f3bcf74a81385"),
                BigInteger256.ParseHexUnsigned("6bc8994d2ff8b18e145821876517018ba9b7c533b221e9dfb639e5bfd55abdc4"),
                BigInteger256.ParseHexUnsigned("6e2c90fc3b29f9af00159d56fcfb4f80f2fea7c4fb96f0a5966664a8931caa3c"),
                curve
            );
            var right = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("d0a9287039b5b1d143e6f6ecf21936dcfb8994292b94fce0f1730f963e82f378"),
                BigInteger256.ParseHexUnsigned("a4f4129df588b6c08ffa58d172c136b52d6f855ee3b15a3e9392fe70d08ea32d"),
                BigInteger256.ParseHexUnsigned("833579bd538e7d654116041a99b4dba37ad1aaa4bef5967137a146afde8e8463"),
                curve
            );
            var result = left - right;
            var expected = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("e8e20737fdb6dbbd482699f8793164304ada8a6eb1ca6fc0d1d10c81f82d0ad1"),
                BigInteger256.ParseHexUnsigned("da5b2c1c24ac81a8f0087b006b500df8758d3314d2b6b71ee3e4971e244fa49d"),
                BigInteger256.ParseHexUnsigned("a9ee70f267e39c0a0309190a6874754949ee2388913b440f27d5fe348b755e9a"),
                curve
            );
            AssertExt.AssertEquals(expected, result);

            //recheck
            AssertExt.AssertEquals(expected.ToAffinePoint(), (left - right).ToAffinePoint());
        }

        [TestCase(
            0,
            "96e6b51d86cc74119556e2dad4965d5f385b8f72ab12613a2d7f3bcf74a81385",
            "6bc8994d2ff8b18e145821876517018ba9b7c533b221e9dfb639e5bfd55abdc4",
            "6e2c90fc3b29f9af00159d56fcfb4f80f2fea7c4fb96f0a5966664a8931caa3c"
            )]
        [TestCase(
            1,
            "0a84e433b730c6c2ab51c57861d3cc139b407e2f1e731d307e374b81e3e77cda",
            "441431e24f66fa8af308d4714505b1a0883295db56683c5fb9c8c42ed8080ad1",
            "5bbd52d7b5b0c5a0a05cb84d4253dc0cc1cde790951ee186504e327ef4c674ce"
            )]
        [TestCase(
            2,
            "4403a906f093d8089297610822ec2f0953eafed2d1ae218300054a99c2b12b65",
            "877ee5a8225b4b6be3d380325734f4ee6d88564ffb9c9f26383ff3178b8b1531",
            "bb7015dcf9a2386ebb63b84a21ba3a8864d1df6045b71e6fc769306536ac50b7"
            )]
        public void ShlTest(int count, string expectedX, string expectedY, string expectedZ) {
            var curve = ECCurve256.Secp256k1;
            var source = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("96e6b51d86cc74119556e2dad4965d5f385b8f72ab12613a2d7f3bcf74a81385"),
                BigInteger256.ParseHexUnsigned("6bc8994d2ff8b18e145821876517018ba9b7c533b221e9dfb639e5bfd55abdc4"),
                BigInteger256.ParseHexUnsigned("6e2c90fc3b29f9af00159d56fcfb4f80f2fea7c4fb96f0a5966664a8931caa3c"),
                curve
            );
            var expected = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned(expectedX),
                BigInteger256.ParseHexUnsigned(expectedY),
                BigInteger256.ParseHexUnsigned(expectedZ),
                curve
            );
            var result = source.ShiftLeft(count);
            AssertExt.AssertEquals(expected, result);

            //recheck
            AssertExt.AssertEquals(expected.ToAffinePoint(), source.ToAffinePoint().ShiftLeft(count));
        }

        [Test]
        public void MulTest() {
            var curve = ECCurve256.Secp256k1;
            var left = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("96e6b51d86cc74119556e2dad4965d5f385b8f72ab12613a2d7f3bcf74a81385"),
                BigInteger256.ParseHexUnsigned("6bc8994d2ff8b18e145821876517018ba9b7c533b221e9dfb639e5bfd55abdc4"),
                BigInteger256.ParseHexUnsigned("6e2c90fc3b29f9af00159d56fcfb4f80f2fea7c4fb96f0a5966664a8931caa3c"),
                curve
            );
            var multiplier = BigInteger256.ParseHexUnsigned("b359fbe006c3016490e0bd17dea2fc13a4ac7a5919cd93e4f90f4b1481cb2d6c");

            var result = left * multiplier;
            var expected = new ECProjectiveMontgomeryPoint256(
                BigInteger256.ParseHexUnsigned("1f8374fb687691e9300f923969edb3db6e297f704502343f80988e46756cd8ff"),
                BigInteger256.ParseHexUnsigned("33b45405ef6cb837ea56eedf6df40bd594445303c6eb2b38e3cef1f3fc0f4d61"),
                BigInteger256.ParseHexUnsigned("395a5188e35b33228ea05927880c49e976e44db74a7fa67e0ecbc703c1e18807"),
                curve
            );
            AssertExt.AssertEquals(expected, result);

            //recheck
            AssertExt.AssertEquals(expected.ToAffinePoint(), (left * multiplier).ToAffinePoint());
        }
    }
}
