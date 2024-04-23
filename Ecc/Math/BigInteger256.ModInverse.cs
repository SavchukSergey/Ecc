namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public readonly BigInteger256 ModInverse(in BigInteger256 modulus) {
            return EuclidExtendedX(this, modulus);
        }

        public readonly BigInteger256 ModInversePrime(in BigInteger256 primeModulus) {
            //Fermat theorem
            return ModPow(primeModulus.Sub(new BigInteger256(2), out var _), primeModulus);
        }

    }
}
