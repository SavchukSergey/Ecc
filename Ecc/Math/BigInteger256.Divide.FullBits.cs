
namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger256 DivRemFullBits(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            var result = new BigInteger256();
            var value = new BigInteger256(0);
            var bit = BITS_SIZE - 1;
            for (var i = 0; i < BITS_SIZE; i++) {
                value.AssignDouble();
                if (dividend.GetBit(bit)) {
                    value.SetBit(0);
                }
                if (value >= divisor) {
                    value.AssignSub(divisor);
                    result.SetBit(bit);
                }
                bit--;
            }
            remainder = value;
            return result;
        }

    }
}