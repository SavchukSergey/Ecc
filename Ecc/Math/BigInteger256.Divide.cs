namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger256 DivRem(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            return DivRemBits(dividend, divisor, out remainder);
        }

        public static BigInteger256 DivRemBits(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            var result = new BigInteger256();
            var value = new BigInteger512(dividend);
            var bit = BITS_SIZE - 1;
            for (var i = 0; i < BITS_SIZE; i++) {
                value.AssignLeftShift();
                if (value.High >= divisor) {
                    value.High.AssignSub(divisor);
                    result.SetBit(bit);
                }
                bit--;
            }
            remainder = value.High;
            return result;
        }

        public static BigInteger256 DivRemNewton(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {

            // [0.5, 1) -> (1, 2]
            var log2 = divisor.Log2();
            var res = divisor.Clone();
            var shift = BITS_SIZE - log2;
            res.AssignLeftShift(shift);
            var divisorFP = new BigInteger512(res);

            var t0 = new BigInteger512(new BigInteger512(48).LeftShiftHalf().ToNative() / new BigInteger256(17).ToNative());
            var t1 = new BigInteger512(new BigInteger512(32).LeftShiftHalf().ToNative() / new BigInteger256(17).ToNative());
            var x0 = t0 - (t1 * divisorFP).Middle;

            var x = x0;
            x.AssignLeftShift(shift);

            var q = x.High;

            remainder = dividend - MulLow(q, divisor);

            return q;
        }
    }
}