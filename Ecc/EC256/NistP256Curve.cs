using Ecc.Math;

namespace Ecc.EC256 {
    public class NistP256Curve : ECCurve256 {

        public const string CURVE_NAME = "nistP256";

        public NistP256Curve() : base(
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

        public override ECPublicKey256 GetPublicKey(in BigInteger256 k) {
            var acc = ECPoint256.Infinity;
            for (var i = 0; i < BigInteger256.BYTES_SIZE; i++) {
                var bt = k.GetByte(i);
                acc += ECPointByteCache256.NistP256.Get(i, bt);
            }
            return new ECPublicKey256(acc);
        }

        public static readonly ECHexInfo HexInfo = new() {
            Name = CURVE_NAME,
            P = "0xffffffff00000001000000000000000000000000ffffffffffffffffffffffff",
            A = "0xffffffff00000001000000000000000000000000fffffffffffffffffffffffc",
            B = "0x5ac635d8aa3a93e7b3ebbd55769886bc651d06b0cc53b0f63bce3c3e27d2604b",
            Gx = "0x6b17d1f2e12c4247f8bce6e563a440f277037d812deb33a0f4a13945d898c296",
            Gy = "0x4fe342e2fe1a7f9b8ee7eb4a7c0f9e162bce33576b315ececbb6406837bf51f5",
            N = "0xffffffff00000000ffffffffffffffffbce6faada7179e84f3b9cac2fc632551",
            H = "0x1"
        };

    }
}
