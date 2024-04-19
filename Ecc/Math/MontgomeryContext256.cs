namespace Ecc.Math {
    public readonly struct MontgomeryContext256 {

        public readonly BigInteger256 Modulus;
        private readonly BigInteger512 _r;
        private readonly BigInteger256 _rm;
        private readonly BigInteger256 _beta; //check actual size. 0 <= beta < r

        public MontgomeryContext256(in BigInteger256 modulus) {
            // k is supported to be 256
            // modulus >= 2 ^ 255
            _r = new BigInteger512(new BigInteger256(0), new BigInteger256(1));
            Modulus = modulus;
            _beta = (_r - new BigInteger512(modulus).ModInverse(_r)).Low;

            _rm = _r % Modulus;
        }

        public readonly BigInteger256 ToMontgomery(in BigInteger256 x) {
            return x.ModMul(_rm, Modulus);
        }

        public BigInteger256 ModMul(BigInteger256 u, BigInteger256 v) {
            var x = u * v;
            return Reduce(x);
        }

        public BigInteger256 ModSquare(BigInteger256 u) {
            var x = u.Square();
            return Reduce(x);
        }

        public readonly BigInteger256 Reduce(in BigInteger512 x) {
            var s1 = x.Low;                 // s1 = x % r
            var s2 = BigInteger256.MulLow(s1, _beta);
            var s3 = Modulus * s2;
            var t = (x + s3).High;
            if (t >= Modulus) {
                t -= Modulus;
            }
            return t;
        }

        public readonly BigInteger256 Reduce(in BigInteger256 x) {
            var s1 = x;                 // s1 = x % r
            var s2 = BigInteger256.MulLow(s1, _beta);
            var s3 = Modulus * s2;
            var t = (s3 + x).High;
            if (t >= Modulus) {
                t -= Modulus;
            }
            return t;
        }

        /*
         * 
        private readonly BigInteger Reduce(in BigInteger x) {
            var s1 = x % _r;              // s1 = x % r
            var s2 = (s1 * _beta) % _r;
            var s3 = Modulus * s2;
            var t = (x + s3) / _r;
            if (t >= Modulus) {
                t -= Modulus;
            }
            return t;
        }

        */

        public bool IsValid {
            get {
                var gcd = BigInteger512.Gcd(_r, Modulus);
                if (!gcd.IsOne) {
                    return false;
                }

                //r * rInv - m * beta = 1

                var rInv = _r.ModInverse(new BigInteger512(Modulus));
                return ((_r * rInv) - (Modulus * _beta)).IsOne;
            }
        }
    }
}
