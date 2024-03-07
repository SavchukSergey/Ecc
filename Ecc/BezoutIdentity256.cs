using System.Numerics;
using Ecc.Math;

namespace Ecc {
    public readonly struct BezoutIdentity256 {

        public readonly BigInteger X { get; init; }

        public readonly BigInteger Y { get; init; }

        public readonly BigInteger256 A { get; init; }

        public readonly BigInteger256 B { get; init; }

        public readonly BigInteger Gcd => A.ToNative() * X + B.ToNative() * Y;

    }
}
