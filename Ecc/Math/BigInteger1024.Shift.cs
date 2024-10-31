using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger1024 {

        [Obsolete]
        public void AssignLeftShiftQuarter() {
            UInt64[15] = UInt64[11];
            UInt64[14] = UInt64[10];
            UInt64[13] = UInt64[9];
            UInt64[12] = UInt64[8];
            UInt64[11] = UInt64[7];
            UInt64[10] = UInt64[6];
            UInt64[9] = UInt64[5];
            UInt64[8] = UInt64[4];

            UInt64[7] = UInt64[3];
            UInt64[6] = UInt64[2];
            UInt64[5] = UInt64[1];
            UInt64[4] = UInt64[0];
            UInt64[3] = 0;
            UInt64[2] = 0;
            UInt64[1] = 0;
            UInt64[0] = 0;
        }

        public void AssignRightShift512() {
            BiLow512 = BiHigh512;
            BiHigh512 = new BigInteger512(0);
        }

        public void AssignRightShift256() {
            for (var i = 0; i < UINT64_SIZE - 4; i++) {
                UInt64[i] = UInt64[i + 4];
            }
            UInt64[UINT64_SIZE - 4] = 0;
            UInt64[UINT64_SIZE - 3] = 0;
            UInt64[UINT64_SIZE - 2] = 0;
            UInt64[UINT64_SIZE - 1] = 0;
        }

        public void AssignRightShift128() {
            for (var i = 0; i < UINT64_SIZE - 2; i++) {
                UInt64[i] = UInt64[i + 2];
            }
            UInt64[UINT64_SIZE - 2] = 0;
            UInt64[UINT64_SIZE - 1] = 0;
        }

        public void AssignRightShift64() {
            for (var i = 0; i < UINT64_SIZE - 1; i++) {
                UInt64[i] = UInt64[i + 1];
            }
            UInt64[UINT64_SIZE - 1] = 0;
        }

        public void AssignRightShift(int count) {
            if (count >= 512) {
                AssignRightShift512();
                count -= 512;
            }
            if (count >= 256) {
                AssignRightShift256();
                count -= 256;
            }
            if (count >= 128) {
                AssignRightShift128();
                count -= 128;
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

    }
}