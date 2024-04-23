using Ecc.Math;

namespace Ecc {
    public readonly struct BezoutIdentity512 {

        public readonly BigInteger512 X { get; init; }

        public readonly BigInteger512 Y { get; init; }

        public readonly BigInteger512 A { get; init; }

        public readonly BigInteger512 B { get; init; }

        public readonly BigInteger512 Gcd => (A * X + B * Y).Low;

    }
}
