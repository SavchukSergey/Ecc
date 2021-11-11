using Ecc.Math;

namespace Ecc {
    public readonly ref struct ECPrivateKey256 {

        public readonly ECCurve Curve { get; init; }

        public readonly BigInteger256 D { get; init; }

    }
}