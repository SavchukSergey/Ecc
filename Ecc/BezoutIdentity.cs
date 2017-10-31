using System.Numerics;

namespace Ecc {
    public struct BezoutIdentity {

        public BigInteger X;

        public BigInteger Y;

        public BigInteger A;

        public BigInteger B;

        public BigInteger Gcd => A * X + B * Y;

    }
}
