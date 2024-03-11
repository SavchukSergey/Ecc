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
            var y = (x0 - one).Low; // 0 < y <= 1, fractional part only

            //todo: limit loop
            for (var i = 0; i < 10; i++) {
                var dyLow = y * divisorFP.Low; // multiply fractional parts
                dyLow.AssignRightShiftHalf(); //  only first 256 of result fraction
                //var dyLowApprox = Mul128(y.High, divisorFP.Low.High); // approx. multiply fractional parts
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
                var ysub = y * subL;  //yf and subL are below 1
                ysub.AssignRightShiftHalf();//  only first 256 of result fraction
                ysub.AssignAdd(sub);

                var deltaX = ysub.Low;
                if (neg) {
                    y.AssignSub(deltaX);
                } else {
                    y.AssignAdd(deltaX);
                }
            }

            var x = new BigInteger512(y, new BigInteger256(0)) + one;
            var reciprocal = x;

            var leftFP = new BigInteger512(new BigInteger256(0), dividend);
            leftFP.AssignRightShift(log2);

            var qfp = BigInteger512.MulFixedPoint(leftFP, reciprocal);
            var q = qfp.High;

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
        public static BigInteger256 DivRem2(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {

            var divShiftBits = divisor.LeadingZeroCount();
            var divisorN64 = divisor.Clone();
            divisorN64.AssignLeftShift(divShiftBits);
            var divisorN63 = divisorN64.Clone();
            divisorN63.AssignRightShift(1);

            var divPart64 = (UInt128)divisorN64.UInt64[3];
            var divPart63 = (UInt128)divisorN63.UInt64[3];

            var q = new BigInteger256();

            var remainderConsumedLog2 = 0;
            remainder = dividend;

            while (remainderConsumedLog2 < BITS_SIZE) {
                var reminderAdjust = remainder.LeadingZeroCount();
                remainderConsumedLog2 += reminderAdjust;
                if (remainderConsumedLog2 > BITS_SIZE) {
                    reminderAdjust -= remainderConsumedLog2 - BITS_SIZE;
                    remainderConsumedLog2 = BITS_SIZE;
                }
                remainder.AssignLeftShift(reminderAdjust);
                if (remainder > divisorN64) {
                    var remPart128 = remainder.High;
                    UInt128 guess = 1; // as reminder >= divissorN64;
                    if (remPart128 > divPart64) {
                        guess = remPart128 / (divPart64 + 1);
                    }
                    var delta = guess * divPart64;
                    remainder.High -= delta;
                    var guessQ = new BigInteger256(guess);
                    var dlog = divShiftBits - remainderConsumedLog2 - 64; // -64 as we take 128 bit reminder versus 64 bit divisor
                    if (dlog > 0) {
                        guessQ.AssignLeftShift(dlog);
                    } else if (dlog < 0) {
                        guessQ.AssignRightShift(-dlog);
                    }
                    q.AssignAdd(guessQ);
                } else {
                    // use 63-bit divisor. Remainder garanteed to be greater that 63-bit divisor
                    var remPart128 = remainder.High;
                    UInt128 guess = 1; // as reminder >= divissorN63;
                    if (remPart128 > divPart63) {
                        guess = remPart128 / (divPart63 + 1);
                    }
                    var delta = guess * divPart63;
                    remainder.High -= delta;
                    var guessQ = new BigInteger256(guess);
                    var dlog = divShiftBits - remainderConsumedLog2 - 65; // -65 as we take 128 bit reminder versus 63 bit divisor
                    if (dlog > 0) {
                        guessQ.AssignLeftShift(dlog);
                    } else if (dlog < 0) {
                        guessQ.AssignRightShift(-dlog);
                    }
                    q.AssignAdd(guessQ);
                }

            }

            remainder.AssignRightShift(189); //magic number. It must be 256

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