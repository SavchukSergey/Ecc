using System;
using System.Numerics;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger256 DivRem(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            return DivRemBits(dividend, divisor, out remainder);
        }

        public static BigInteger256 DivRemBits(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            var divLZC = divisor.LeadingZeroCount();
            if (divLZC == BITS_SIZE) {
                throw new DivideByZeroException();
            }
            var divisorN = divisor;
            divisorN.AssignLeftShift(divLZC);

            var result = new BigInteger256();
            var value = dividend;
            var bit = divLZC;
            var totalShift = 0;

            while (bit >= 0) {
                var valueLZC = value.LeadingZeroCount();
                bit -= valueLZC;
                if (bit < 0) {
                    break;
                }

                value.AssignLeftShift(valueLZC);
                totalShift += valueLZC;

                if (value >= divisorN) {
                    value.AssignSub(divisorN);
                    result.SetBit(bit);
                } else {
                    //next shift guranteed to be successful
                    if (bit > 0) {
                        value.AssignDouble(); // overflow to 257 bit but it does not matter
                        totalShift++;
                        value.AssignSub(divisorN);
                        result.SetBit(bit - 1);
                    }
                    bit--;
                }
            }
            remainder = value.RightShift(totalShift);
            return result;
        }

        public static BigInteger256 DivRemFullBits(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            var result = new BigInteger256();
            var value = new BigInteger512(dividend);
            var bit = BITS_SIZE - 1;
            for (var i = 0; i < BITS_SIZE; i++) {
                value.AssignDouble();
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
            var log2 = divisor.LeadingZeroCount();
            var divisor256 = divisor.Clone();
            divisor256.AssignLeftShift(log2);
            var divisorFP = new BigInteger512(divisor256);

            //todo: special case 0.5

            var x0 = EstimateReciprocal(divisorFP);

            var y = x0.Low; // 0 < y <= 1, fractional part only

            //todo: limit loop
            for (var i = 0; i < 5; i++) {
                var dyLow256 = MulHigh(y, divisorFP.Low);// multiply fractional parts and use only first 256 bits of result fraction

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

            var x = new BigInteger512(y, new BigInteger256(0));
            x.High.AssignIncrement();
            var reciprocal = x;

            var leftFP = new BigInteger512(new BigInteger256(0), dividend);
            leftFP.AssignRightShift(BITS_SIZE - log2);

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

        //todo: use it?
        public static BigInteger256 DivRem2(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {

            var divShiftBits = divisor.LeadingZeroCount();
            if (divShiftBits >= BITS_SIZE - 32) {
                return DivRem32(dividend, divisor.UInt32[0], out remainder);
            }
            if (divShiftBits >= BITS_SIZE - 64) {
                return DivRem64(dividend, divisor.UInt64[0], out remainder);
            }

            var q = new BigInteger256();

            var originalDividend = dividend.Clone();

            var dividend2 = dividend;
            //todo: check divisor.UInt64[3] + 1 overflow
            //todo: check if divisor.UInt64[3] == 0, useless estiamtion
            //todo: normalzie divisor before estimate?
            //var estimate = DivRem64(dividend2, divisor.UInt64[3] + 1, out var _);
            //estimate.AssignRightShift(192);
            //var estimateQ = estimate * divisor;
            //q.AssignAdd(estimate);
            //dividend2.AssignSub(estimateQ.Low);

            var divisorN = divisor.Clone();
            divisorN.AssignLeftShift(divShiftBits);

            var divPart64 = (UInt128)divisorN.UInt64[3];

            var remainderConsumedLog2 = 0;
            remainder = dividend2;

            //todo: estimate division by divisor rounded by highest 64 bits. and then refine with expensive routine below
            //todo: it performs better with division by byte rather then u64

            while (remainderConsumedLog2 < BITS_SIZE) {
                var remainderAdjust = remainder.LeadingZeroCount();
                remainderConsumedLog2 += remainderAdjust;
                if (remainderConsumedLog2 > BITS_SIZE) {
                    remainderAdjust -= remainderConsumedLog2 - BITS_SIZE;
                    remainderConsumedLog2 = BITS_SIZE;
                }
                remainder.AssignLeftShift(remainderAdjust);
                var remPart128 = remainder.High;

                UInt128 guess = remPart128 / (divPart64 + 1);

                var delta = divisor * guess;
                var correction2 = divShiftBits - 64; // -64 as we take 128 bit remainder versus 64 bit divisor
                if (correction2 > 0) {
                    delta.AssignLeftShift(correction2);
                } else if (correction2 < 0) {
                    delta.AssignRightShift(-correction2);
                }
                remainder.AssignSub(delta.Low);
                var correction = divShiftBits - remainderConsumedLog2 - 64; // -64 as we take 128 bit remainder versus 64 bit divisor
                var guessQ = new BigInteger256(guess);
                if (correction > 0) {
                    guessQ.AssignLeftShift(correction);
                } else if (correction < 0) {
                    guessQ.AssignRightShift(-correction);
                }
                q.AssignAdd(guessQ);
            }

            remainder = originalDividend - (divisor * q).Low; //todo: it could be derived somehow from above

            return q;
        }

        public static BigInteger256 DivRem64(in BigInteger256 dividend, ulong divisor, out BigInteger256 remainder) {
            //todo: very fast at first bits but slow at then end. Get first half and improve by Newton?
            var divShiftBits = BitOperations.LeadingZeroCount(divisor);
            //todo: if divisor is small call optimized method
            var divisorN = divisor << divShiftBits;

            var divPart128 = (UInt128)divisorN;

            var q = new BigInteger256();

            var remainderConsumedLog2 = 0;
            remainder = dividend;

            //todo: estimate division by divisor rounded by highest 32 bits. and then refine with expensive routine below
            //todo: it performs better with division by byte rather then u64

            while (remainderConsumedLog2 < BITS_SIZE) {
                var remainderAdjust = remainder.LeadingZeroCount();
                remainderConsumedLog2 += remainderAdjust;
                if (remainderConsumedLog2 > BITS_SIZE) {
                    remainderAdjust -= remainderConsumedLog2 - BITS_SIZE;
                    remainderConsumedLog2 = BITS_SIZE;
                }
                remainder.AssignLeftShift(remainderAdjust);
                var remPart128 = remainder.High;

                UInt128 guess = remPart128 / (divPart128 + 1);

                var delta = divisorN * guess;
                remainder.High -= delta;

                var guessQ = new BigInteger256(guess);
                var dlog = divShiftBits - remainderConsumedLog2 + 128;
                if (dlog > 0) {
                    guessQ.AssignLeftShift(dlog);
                } else if (dlog < 0) {
                    guessQ.AssignRightShift(-dlog);
                }
                q.AssignAdd(guessQ);
            }

            remainder = dividend - (q * divisor).Low; //todo: it could be derived somehow from above

            return q;
        }

        public static BigInteger256 DivRem32(in BigInteger256 dividend, uint divisor, out BigInteger256 remainder) {
            //todo: very fast at first bits but slow at then end. Get first half and improve by Newton?
            var divShiftBits = BitOperations.LeadingZeroCount(divisor);
            //todo: if divisor is small call optimized method
            var divisorN = divisor << divShiftBits;

            var divPart128 = (UInt128)divisorN;

            var q = new BigInteger256();

            var remainderConsumedLog2 = 0;
            remainder = dividend;

            //todo: estimate division by divisor rounded by highest 16 bits. and then refine with expensive routine below
            //todo: it performs better with division by byte rather then u64

            while (remainderConsumedLog2 < BITS_SIZE) {
                var remainderAdjust = remainder.LeadingZeroCount();
                remainderConsumedLog2 += remainderAdjust;
                if (remainderConsumedLog2 > BITS_SIZE) {
                    remainderAdjust -= remainderConsumedLog2 - BITS_SIZE;
                    remainderConsumedLog2 = BITS_SIZE;
                }
                remainder.AssignLeftShift(remainderAdjust);
                var remPart128 = remainder.High;

                UInt128 guess = remPart128 / (divPart128 + 1);

                var delta = divisorN * guess;
                remainder.High -= delta;

                var guessQ = new BigInteger256(guess);
                var dlog = divShiftBits - remainderConsumedLog2 + 128;
                if (dlog > 0) {
                    guessQ.AssignLeftShift(dlog);
                } else if (dlog < 0) {
                    guessQ.AssignRightShift(-dlog);
                }
                q.AssignAdd(guessQ);
            }

            remainder = dividend - (q * divisor).Low; //todo: it could be derived somehow from above

            if (remainder >= divisor) {
                remainder.AssignSub(new BigInteger256(divisor));
                q.AssignIncrement();
            }

            return q;
        }

        private static BigInteger512 EstimateReciprocal(in BigInteger512 divisorFP) {
            var tableIndex = divisorFP.Low.High >> 64;
            var max = new UInt128(0x8000_0000_0000_0000ul, 0); // 2 ^ 127
            var value = max / tableIndex;

            //todo: LERP between two neighbour values will double valid bits

            return new BigInteger512(new BigInteger256(value), new BigInteger256()).LeftShift(193);
        }
    }
}