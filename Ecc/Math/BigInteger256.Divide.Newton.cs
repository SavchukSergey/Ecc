
using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger256 DivRemNewton(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {

            // [0.5, 1) -> (1, 2]
            var log2 = divisor.LeadingZeroCount();
            var divisor256 = divisor.Clone();
            divisor256.AssignLeftShift(log2);

            //todo: special case 0.5

            var x0 = EstimateReciprocal(divisor256);

            var y = x0.Low; // 0 < y <= 1, fractional part only

            //todo: limit loop
            for (var i = 0; i < 5; i++) {
                var dyLow256 = MulHigh(y, divisor256);// multiply fractional parts and use only first 256 bits of result fraction

                var dx2 = dyLow256;
                dx2.AssignAdd(divisor256); //sum is >= 1.0
                if (dx2.IsZero) {
                    break;
                }
                dx2.AssignNegate();

                var ysub256 = MulHigh(y, dx2);  //y and subL are below 1 and use only first 256 bits of result fraction
                y.AssignAdd(dx2);
                y.AssignAdd(ysub256);
            }

            var t = new BigInteger1024(y * dividend);
            t.Middle.AssignAdd(dividend);
            t.AssignRightShift(BITS_SIZE - log2);
            var q = t.Low.High;

            remainder = dividend - MulLow(q, divisor);
            if (remainder >= divisor) {
                remainder.AssignSub(divisor);
                q.AssignDecrement();
                if (remainder >= divisor) {
                    remainder.AssignSub(divisor);
                    q.AssignDecrement();
                }
            }

            return q;
        }

        private static BigInteger512 EstimateReciprocal(in BigInteger256 divisor256) {
            var tableIndex = divisor256.High >> 64;
            var max = new UInt128(0x8000_0000_0000_0000ul, 0); // 2 ^ 127
            var value = max / tableIndex;

            //todo: LERP between two neighbour values will double valid bits

            return new BigInteger512(new BigInteger256(value), new BigInteger256()).LeftShift(193);
        }

    }
}