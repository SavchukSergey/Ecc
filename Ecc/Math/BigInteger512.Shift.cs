using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public void AssignLeftShift32() {
            UInt32[15] = UInt32[14];
            UInt32[14] = UInt32[13];
            UInt32[13] = UInt32[12];
            UInt32[12] = UInt32[11];
            UInt32[11] = UInt32[10];
            UInt32[10] = UInt32[9];
            UInt32[9] = UInt32[8];
            UInt32[8] = UInt32[7];
            UInt32[7] = UInt32[6];
            UInt32[6] = UInt32[5];
            UInt32[5] = UInt32[4];
            UInt32[4] = UInt32[3];
            UInt32[3] = UInt32[2];
            UInt32[2] = UInt32[1];
            UInt32[1] = UInt32[0];
            UInt32[0] = 0;
        }

        public void AssignLeftShift64() {
            UInt64[7] = UInt64[6];
            UInt64[6] = UInt64[5];
            UInt64[5] = UInt64[4];
            UInt64[4] = UInt64[3];
            UInt64[3] = UInt64[2];
            UInt64[2] = UInt64[1];
            UInt64[1] = UInt64[0];
            UInt64[0] = 0;
        }

        public void AssignLeftShiftQuarter() {
            UInt64[7] = UInt64[5];
            UInt64[6] = UInt64[4];
            UInt64[5] = UInt64[3];
            UInt64[4] = UInt64[2];
            UInt64[3] = UInt64[1];
            UInt64[2] = UInt64[0];
            UInt64[1] = 0;
            UInt64[0] = 0;
        }

        public void AssignLeftShiftHalf() {
            High = Low;
            Low = new BigInteger256(0);
        }

        public readonly BigInteger512 LeftShiftHalf() {
            return new BigInteger512(new BigInteger256(0), Low);
        }

        public void AssignLeftShift(int count) {
            if (count >= BITS_SIZE / 2) {
                AssignLeftShiftHalf();
                count -= BITS_SIZE / 2;
            }
            if (count >= BITS_SIZE / 4) {
                AssignLeftShiftQuarter();
                count -= BITS_SIZE / 4;
            }
            if (count >= 64) {
                AssignLeftShift64();
                count -= 64;
            }
            //if (count >= 32) {
            //    AssignLeftShift32();
            //    count -= 32;
            //}
            ulong carry = 0;
            var restBits = 64 - count;
            for (var i = 0; i < UINT64_SIZE; i++) {
                var acc = UInt64[i];
                UInt64[i] = (acc << count) + carry;
                carry = acc >> restBits;
            }
        }
    }
}