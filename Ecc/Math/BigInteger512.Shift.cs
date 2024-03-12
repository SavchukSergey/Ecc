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

        public void AssignRightShift32() {
            UInt32[0] = UInt32[1];
            UInt32[1] = UInt32[2];
            UInt32[2] = UInt32[3];
            UInt32[3] = UInt32[4];
            UInt32[4] = UInt32[5];
            UInt32[5] = UInt32[6];
            UInt32[6] = UInt32[7];
            UInt32[7] = UInt32[8];
            UInt32[8] = UInt32[9];
            UInt32[9] = UInt32[10];
            UInt32[10] = UInt32[11];
            UInt32[11] = UInt32[12];
            UInt32[12] = UInt32[13];
            UInt32[13] = UInt32[14];
            UInt32[14] = UInt32[15];
            UInt32[15] = 0;
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

        public void AssignRightShift64() {
            UInt64[0] = UInt64[1];
            UInt64[1] = UInt64[2];
            UInt64[2] = UInt64[3];
            UInt64[3] = UInt64[4];
            UInt64[4] = UInt64[5];
            UInt64[5] = UInt64[6];
            UInt64[6] = UInt64[7];
            UInt64[7] = 0;
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

        public void AssignRightShiftQuarter() {
            UInt64[0] = UInt64[2];
            UInt64[1] = UInt64[3];
            UInt64[2] = UInt64[4];
            UInt64[3] = UInt64[5];
            UInt64[4] = UInt64[6];
            UInt64[5] = UInt64[7];
            UInt64[6] = 0;
            UInt64[7] = 0;
        }

        public void AssignLeftShiftHalf() {
            High = Low;
            Low = new BigInteger256(0);
        }

        public void AssignRightShiftHalf() {
            Low = High;
            High = new BigInteger256(0);
        }

        public readonly BigInteger512 LeftShiftHalf() {
            return new BigInteger512(new BigInteger256(0), Low);
        }

        public readonly BigInteger512 RightShiftHalf() {
            return new BigInteger512(High, new BigInteger256(0));
        }

        public uint ShiftLeft() {
            uint carry = 0;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                var sum = (ulong)carry;
                sum += Data[i];
                sum += Data[i];
                Data[i] = (uint)sum;
                carry = (uint)(sum >> 32);
            }
            return carry;
        }

        public BigInteger512 LeftShift(int count) {
            var res = new BigInteger512(this);
            res.AssignLeftShift(count);
            return res;
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
            if (count > 0) {
                ulong carry = 0;
                var restBits = 64 - count;
                for (var i = 0; i < UINT64_SIZE; i++) {
                    var acc = UInt64[i];
                    UInt64[i] = (acc << count) + carry;
                    carry = acc >> restBits;
                }
            }
        }

        public void AssignRightShift(int count) {
            if (count >= BITS_SIZE / 2) {
                AssignRightShiftHalf();
                count -= BITS_SIZE / 2;
            }
            if (count >= BITS_SIZE / 4) {
                AssignRightShiftQuarter();
                count -= BITS_SIZE / 4;
            }
            if (count >= 64) {
                AssignRightShift64();
                count -= 64;
            }
            if (count > 0) {
                ulong carry = 0;
                var restBits = 64 - count;
                for (var i = UINT64_SIZE - 1; i >= 0; i--) {
                    var acc = UInt64[i];
                    UInt64[i] = (acc >> count) + carry;
                    carry = acc << restBits;
                }
            }
        }

        public readonly BigInteger512 RightShift(int count) {
            var res = new BigInteger512(this);
            res.AssignRightShift(count);
            return res;
        }
    }
}