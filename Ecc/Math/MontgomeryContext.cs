using System.Numerics;

namespace Ecc.Math {
    public readonly struct MontgomeryContext {

        public readonly BigInteger Modulus;
        private readonly BigInteger _r;
        private readonly BigInteger _rm;
        private readonly BigInteger _beta;

        public MontgomeryContext(in BigInteger modulus) {
            var k = modulus.GetBitLength();
            _r = new BigInteger(1) << (int)k;
            Modulus = modulus;
            _beta = _r - Modulus.ModInverse(_r);

            _rm = _r % Modulus;
        }

        public readonly BigInteger ToMontgomery(in BigInteger x) {
            return x.ModMul(_rm, Modulus);
        }

        public readonly BigInteger ModMul(in BigInteger u, in BigInteger v) {
            var x = u * v;
            return Reduce(x);
        }

        public readonly BigInteger ModSquare(in BigInteger u) {
            var x = u * u;
            return Reduce(x);
        }

        public readonly BigInteger Reduce(in BigInteger x) {
            //todo: precalc beta * modulus?
            var s1 = x % _r;              // s1 = x % r
            var s2 = (s1 * _beta) % _r;
            var s3 = Modulus * s2;
            var t = (x + s3) / _r;
            if (t >= Modulus) {
                t -= Modulus;
            }
            return t;
        }

        public bool IsValid {
            get {
                var gcd = BigInteger.GreatestCommonDivisor(_r, Modulus);
                if (!gcd.IsOne) {
                    return false;
                }

                //r * rInv - m * beta = 1

                var rInv = _r.ModInverse(Modulus);
                return ((_r * rInv) - (Modulus * _beta)).IsOne;
            }
        }
    }
}
