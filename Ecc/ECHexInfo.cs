using System.Numerics;

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
                A = ParseInt(A),
                B = ParseInt(B),
                Modulus = ParseInt(P),
                Order = ParseInt(N),
                Cofactor = ParseInt(H),
            };
            ecc.G = new ECPoint {
                X = ParseInt(Gx),
                Y = ParseInt(Gy),
                Curve = ecc
            };
            return ecc;
        }

        private static BigInteger ParseInt(string val) {
            if (val.StartsWith("0x")) val = "0" + val.Substring(2);
            return BigInteger.Parse(val, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

    }

}
