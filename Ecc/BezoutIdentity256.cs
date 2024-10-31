using Ecc.Math;

namespace Ecc {
    public readonly struct BezoutIdentity256 {

        public readonly BigInteger256 X { get; init; }

        public readonly BigInteger256 Y { get; init; }

        public readonly BigInteger256 A { get; init; }

        public readonly BigInteger256 B { get; init; }

        public readonly BigInteger256 Gcd => (A * X + B * Y).BiLow256;

    }
}
