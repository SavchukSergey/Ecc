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

        public void AssignLeftShift128() {
            UInt128_3 = UInt128_2;
            UInt128_2 = UInt128_1;
            UInt128_1 = UInt128_0;
            UInt128_0 = UInt128.Zero;
        }

        public void AssignRightShift128() {
            UInt128_0 = UInt128_1;
            UInt128_1 = UInt128_2;
            UInt128_2 = UInt128_3;
            UInt128_3 = UInt128.Zero;
        }

        public void AssignRightShiftQuarter() {
            UInt128_0 = UInt128_1;
            UInt128_1 = UInt128_2;
            UInt128_2 = UInt128_3;
            UInt128_3 = UInt128.Zero;
        }

        public void AssignLeftShift256() {
            BiHigh256 = BiLow256;
            BiLow256.Clear();
        }

        public void AssignRightShift256() {
            BiLow256 = BiHigh256;
            BiHigh256.Clear();
        }

        public readonly BigInteger512 LeftShift256() {
            return new BigInteger512(new BigInteger256(0), BiLow256);
        }

        public readonly BigInteger512 RightShift256() {
            return new BigInteger512(BiHigh256, new BigInteger256(0));
        }

        public BigInteger512 LeftShift(int count) {
            var res = new BigInteger512(this);
            res.AssignLeftShift(count);
            return res;
        }

        public void AssignLeftShift(int count) {
            if (count >= 256) {
                AssignLeftShift256();
                count -= BITS_SIZE / 2;
            }
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

        public void AssignRightShift(int count) {
            if (count >= BITS_SIZE / 2) {
                AssignRightShift256();
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

        public static BigInteger512 operator <<(in BigInteger512 value, int count) {
            var res = new BigInteger512(value);
            res.AssignLeftShift(count);
            return res;
        }

        public readonly BigInteger128 ExtractHigh128(int skipCount) {
            if (skipCount >= 512) {
                return new BigInteger128();
            }
            if (skipCount >= 256) {
                return BiLow256.ExtractHigh128(skipCount - 256);
            }
            //todo:
            return (this << skipCount).BiHigh128;
        }

        public readonly ulong ExtractHigh64(int skipCount) {
            if (skipCount >= 512) {
                return 0;
            }
            if (skipCount >= 256) {
                return BiLow256.ExtractHigh64(skipCount - 256);
            }

            //todo:
            return (this << skipCount).HighUInt64;
        }
    }
}