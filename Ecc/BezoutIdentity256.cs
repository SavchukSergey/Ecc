using System.Numerics;

namespace Ecc {
    public readonly struct BezoutIdentity256 {

        public readonly BigInteger X { get; init; }

        public readonly BigInteger Y { get; init; }

        public readonly BigInteger A { get; init; }

        public readonly BigInteger B { get; init; }

        public readonly BigInteger Gcd => A * X + B * Y;

    }
}
