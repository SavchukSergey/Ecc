using Ecc.Math;

namespace Ecc {
    public readonly ref struct ECPrivateKey512 {

        public readonly ECCurve Curve { get; init; }

        public readonly BigInteger512 D { get; init; }

    }
}