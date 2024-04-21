
using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BigInteger256 DivRemBits(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            var divLZC = divisor.LeadingZeroCount();
            if (divLZC == BITS_SIZE) {
                throw new DivideByZeroException();
            }
            var divisorN = divisor;
            divisorN.AssignLeftShift(divLZC);

            var quotient = new BigInteger256();
            remainder = dividend;
            var bit = divLZC;
            var totalShift = 0;

            while (true) {
                var valueLZC = remainder.LeadingZeroCount();
                bit -= valueLZC;
                if (bit < 0) {
                    break;
                }

                remainder.AssignLeftShift(valueLZC);
                totalShift += valueLZC;

                if (remainder >= divisorN) {
                    remainder.AssignSub(divisorN);
                    quotient.SetBit(bit);
                } else {
                    if (bit == 0) {
                        break;
                    }
                    bit--;

                    //next shift guranteed to be successful
                    remainder.AssignDouble(); // overflow to 257 bit but it does not matter
                    totalShift++;
                    remainder.AssignSub(divisorN);
                    quotient.SetBit(bit);
                }
            }
            remainder.AssignRightShift(totalShift);
            return quotient;
        }

    }
}