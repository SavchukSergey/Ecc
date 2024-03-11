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
            res.AssignLeftShift(BITS_SIZE - log2);
            var divisorFP = new BigInteger512(res);

            //todo: use lookup table for x0 by first hex digit

            // 48/17
            var divideT0FP = new BigInteger512(
                new BigInteger256(0xd2d2d2d2d2d2d2d2ul, 0xd2d2d2d2d2d2d2d2ul, 0xd2d2d2d2d2d2d2d2ul, 0xd2d2d2d2d2d2d2d2ul),
                new BigInteger256(2)
            );
            // 32 / 17
            var divideT1FP = new BigInteger512(
                new BigInteger256(0xe1e1e1e1e1e1e1e1ul, 0xe1e1e1e1e1e1e1e1ul, 0xe1e1e1e1e1e1e1e1ul, 0xe1e1e1e1e1e1e1e1ul),
                new BigInteger256(1)
            );

            var x0 = divideT0FP - BigInteger512.MulFixedPoint(divideT1FP, divisorFP);

            var x = x0;

            var one = new BigInteger512(new BigInteger256(0), new BigInteger256(1));
            //todo: limit loop
            for (var i = 0; i < 10; i++) {
                var sub = one.Sub(BigInteger512.MulFixedPoint(x, divisorFP), out var neg);
                if (neg) {
                    sub.AssignNegate();
                }
                var dx = BigInteger512.MulFixedPoint(x, sub);
                if (dx.IsZero) {
                    break;
                }
                if (neg) {
                    x.AssignSub(dx);
                } else {
                    x.AssignAdd(dx);
                }
            }

            var reciprocal = x;

            var leftFP = new BigInteger512(new BigInteger256(0), dividend);
            leftFP.AssignRightShift(log2);

            var qfp = BigInteger512.MulFixedPoint(leftFP, reciprocal);
            var q = qfp.High;

            //todo: above is just appriximation. use while to clarify

            remainder = dividend - MulLow(q, divisor);

            return q;
        }

    }
}