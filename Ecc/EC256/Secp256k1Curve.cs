using Ecc.Math;

namespace Ecc.EC256 {
    public class Secp256k1Curve : ECCurve256 {

        public const string CURVE_NAME = "secp256k1";

        public Secp256k1Curve() : base(
            CURVE_NAME,
            BigInteger256.ParseHexUnsigned(HexInfo.A),
            BigInteger256.ParseHexUnsigned(HexInfo.B),
            BigInteger256.ParseHexUnsigned(HexInfo.P),
            BigInteger256.ParseHexUnsigned(HexInfo.N),
            BigInteger256.ParseHexUnsigned(HexInfo.H),
            BigInteger256.ParseHexUnsigned(HexInfo.Gx),
            BigInteger256.ParseHexUnsigned(HexInfo.Gy)
        ) {
        }

        public override ECPoint256 MulG(in BigInteger256 k) {
            var acc = new ECProjectiveMontgomeryPoint256(
                new BigInteger256(0),
                new BigInteger256(0),
                this
            );
            for (var i = 0; i < BigInteger256.BYTES_SIZE; i++) {
                var bt = k.GetByte(i);
                acc += ECProjectiveMontgomeryPointByteCache256.Secp256k1.Get(i, bt);
            }
            return acc.Reduce().ToAffinePoint();
        }

        public static readonly ECHexInfo HexInfo = new() {
            Name = CURVE_NAME,
            P = "0xfffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f",
            A = "0x0",
            B = "0x7",
            Gx = "0x79be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798",
            Gy = "0x483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8",
            N = "0xfffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141",
            H = "0x1"
        };

    }
}
