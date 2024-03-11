using Ecc.Math;

namespace Ecc {
    public struct ECHexInfo {

        public string Name;

        public string P;

        public string A;

        public string B;

        public string Gx;

        public string Gy;

        public string N;

        public string H;

        public readonly ECCurve Build() {
            var ecc = new ECCurve(
                name: Name,
                a: BigIntegerExt.ParseHexUnsigned(A),
                b: BigIntegerExt.ParseHexUnsigned(B),
                modulus: BigIntegerExt.ParseHexUnsigned(P),
                order: BigIntegerExt.ParseHexUnsigned(N),
                cofactor: BigIntegerExt.ParseHexUnsigned(H),
                gx: BigIntegerExt.ParseHexUnsigned(Gx),
                gy: BigIntegerExt.ParseHexUnsigned(Gy)
            );
            return ecc;
        }

        public readonly ECCurve256 Build256() {
            var ecc = new ECCurve256(
                name: Name,
                a: BigInteger256.ParseHexUnsigned(A),
                b: BigInteger256.ParseHexUnsigned(B),
                modulus: BigInteger256.ParseHexUnsigned(P),
                order: BigInteger256.ParseHexUnsigned(N),
                cofactor: BigInteger256.ParseHexUnsigned(H),
                gx: BigInteger256.ParseHexUnsigned(Gx),
                gy: BigInteger256.ParseHexUnsigned(Gy)
            );
            return ecc;
        }
    }

}
