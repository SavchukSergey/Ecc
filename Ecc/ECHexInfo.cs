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

        public ECCurve Build() {
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

    }

}
