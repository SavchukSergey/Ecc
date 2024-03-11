using System;
using System.Numerics;

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

            //todo: special case 0.5

            var x0 = EstimateReciprocal(divisorFP);


            var one = new BigInteger512(new BigInteger256(0), new BigInteger256(1));
            var y = x0 - one; // 0 < y <= 1

            //todo: limit loop
            for (var i = 0; i < 10; i++) {
                var yf = y.Low; // fraction of Y
                var dyLow = yf * divisorFP.Low; // multiply fractional parts
                dyLow.AssignRightShiftHalf(); //  only first 256 of result fraction
                //var dyLowApprox = Mul128(yf.High, divisorFP.Low.High); // approx. multiply fractional parts
                //var dyLow = new BigInteger512(dyLowApprox); //  only first 256 of result fraction

                var dx = dyLow + divisorFP;
                var sub = one.Sub(dx, out var neg);
                if (sub.IsZero) {
                    break;
                }
                if (neg) {
                    sub.AssignNegate();
                }
                var subL = sub.Low;
                var ysub = yf * subL;  //yf and subL are below 1
                ysub.AssignRightShiftHalf();//  only first 256 of result fraction
                ysub.AssignAdd(sub);

                var deltaX = ysub; // BigInteger512.MulFixedPoint(x, sub);
                if (neg) {
                    y.AssignSub(deltaX);
                } else {
                    y.AssignAdd(deltaX);
                }
            }

            var x = y + one;
            var reciprocal = x;

            var leftFP = new BigInteger512(new BigInteger256(0), dividend);
            leftFP.AssignRightShift(log2);

            var qfp = BigInteger512.MulFixedPoint(leftFP, reciprocal);
            var q = qfp.High;

            //todo: above is just appriximation. use while to clarify

            remainder = dividend - MulLow(q, divisor);

            return q;
        }


        private static BigInteger512 EstimateReciprocal(in BigInteger512 divisorFP) {
            var tableIndex = divisorFP.Low.High >> 64;
            var max = new UInt128(0x8000_0000_0000_0000ul, 0); // 2 ^ 127
            var value = max / tableIndex; 

            return new BigInteger512(new BigInteger256(value), new BigInteger256()).LeftShift(193);
        }
    }
}