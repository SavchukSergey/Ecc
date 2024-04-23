namespace Ecc.Math {
    public readonly struct MontgomeryContext256 {

        public readonly BigInteger256 Modulus;
        private readonly BigInteger512 _r;
        private readonly BigInteger256 _rm;
        private readonly BigInteger256 _beta;
        public readonly BigInteger256 One;

        public MontgomeryContext256(in BigInteger256 modulus) {
            // k is supposed to be 256
            // modulus >= 2 ^ 255, odd
            _r = new BigInteger512(new BigInteger256(0), new BigInteger256(1));
            Modulus = modulus;
            _beta = (_r - new BigInteger512(modulus).ModInverse(_r)).Low;

            _rm = _r % Modulus;

            One = _rm;
        }

        public readonly BigInteger256 ToMontgomery(in BigInteger256 x) {
            return x.ModMul(_rm, Modulus);
        }

        public readonly BigInteger256 ModMul(in BigInteger256 u, in BigInteger256 v) {
            var x = u * v;
            return Reduce(x);
        }

        public readonly BigInteger256 ModSquare(in BigInteger256 u) {
            var x = u.Square();
            return Reduce(x);
        }

        public readonly BigInteger256 Reduce(in BigInteger512 x) {
            //todo: precalc beta * modulus?
            var s1 = x.Low;                 // s1 = x % r
            var s2 = BigInteger256.MulLow(s1, _beta);
            var s3 = Modulus * s2;
            // x + s3 requires extra one bit precission
            var carry = s3.AssignAdd(x);
            var t = s3.High;
            if (carry || t >= Modulus) {
                t -= Modulus;
            }
            return new BigInteger256(t);
        }

        public readonly BigInteger256 Reduce(in BigInteger256 x) {
            //todo: precalc beta * modulus?
            var s1 = x;                 // s1 = x % r
            var s2 = BigInteger256.MulLow(s1, _beta);
            var s3 = Modulus * s2;

            // x + s3 requires extra one bit precission
            var carry = s3.AssignAdd(x);
            var t = s3.High;
            if (carry || t >= Modulus) {
                t -= Modulus;
            }
            return new BigInteger256(t);
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
