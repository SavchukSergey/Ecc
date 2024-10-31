using System.Numerics;

namespace Ecc.Math {
    public readonly struct BezoutIdentity {

        public readonly BigInteger X { get; init; }

        public readonly BigInteger Y { get; init; }

        public readonly BigInteger A { get; init; }

        public readonly BigInteger B { get; init; }

        public readonly BigInteger Gcd => A * X + B * Y;

    }
}
