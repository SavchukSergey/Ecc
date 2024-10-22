using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public void AssignLeftShift8() {
            High = (High << 8) + BiLow.HighByte;
            BiLow.AssignLeftShift8();
        }

        public void AssignLeftShift16() {
            High = (High << 16) + BiLow.HighUInt16;
            BiLow.AssignLeftShift16();
        }

        public void AssignLeftShift32() {
            High = (High << 32) + BiLow.HighUInt32;
            BiLow.AssignLeftShift32();
        }

        public void AssignLeftShift64() {
            UInt64[3] = UInt64[2];
            UInt64[2] = UInt64[1];
            UInt64[1] = UInt64[0];
            UInt64[0] = 0;
        }

        public void AssignLeftShift128() {
            High = Low;
            Low = UInt128.Zero;
        }

        public void AssignLeftShiftQuarter() {
            UInt64[3] = UInt64[2];
            UInt64[2] = UInt64[1];
            UInt64[1] = UInt64[0];
            UInt64[0] = 0;
        }

        public void AssignLeftShiftHalf() {
            High = Low;
            Low = 0;
        }

        public void AssignLeftShift(int count) {
            if (count >= 128) {
                AssignLeftShift128();
                count -= 128;
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

        public void AssignRightShiftHalf() {
            Low = High;
            High = 0;
        }

        public void AssignRightShiftQuarter() {
            UInt64[0] = UInt64[1];
            UInt64[1] = UInt64[2];
            UInt64[2] = UInt64[3];
            UInt64[3] = 0;
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

        public readonly BigInteger256 RightShift(int count) {
            var res = new BigInteger256(this);
            res.AssignRightShift(count);
            return res;
        }

        public static BigInteger256 operator <<(in BigInteger256 value, int count) {
            var res = new BigInteger256(value);
            res.AssignLeftShift(count);
            return res;
        }
    }
}