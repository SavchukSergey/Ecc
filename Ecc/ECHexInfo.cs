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
            var ecc = new ECCurve {
                Name = Name,
                A = BigIntegerExt.ParseHexUnsigned(A),
                B = BigIntegerExt.ParseHexUnsigned(B),
                Modulus = BigIntegerExt.ParseHexUnsigned(P),
                Order = BigIntegerExt.ParseHexUnsigned(N),
                Cofactor = BigIntegerExt.ParseHexUnsigned(H),
            };
            var g = ecc.CreatePoint(BigIntegerExt.ParseHexUnsigned(Gx), BigIntegerExt.ParseHexUnsigned(Gy));
            ecc.G = g;
            return ecc;
        }

    }

}
