using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public void AssignLeftShift8() {
            High = (High << 8) + (Low >> (128 - 8));
            Low <<= 8;
        }

        public void AssignLeftShift16() {
            High = (High << 16) + (Low >> (128 - 16));
            Low <<= 16;
        }

        public void AssignLeftShift32() {
            High = (High << 32) + (Low >> (128 - 32));
            Low <<= 32;
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
            High = (High << count) + (Low >> (128 - count));
            Low <<= count;
        }

    }
}