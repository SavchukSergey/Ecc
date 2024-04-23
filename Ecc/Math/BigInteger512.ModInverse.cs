namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public readonly BigInteger512 ModInverse(in BigInteger512 modulus) {
            return EuclidExtendedX(this, modulus);
        }

    }
}
