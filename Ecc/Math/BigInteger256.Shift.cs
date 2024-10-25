using System;
using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignLeftShiftHalf() {
            High = Low;
            Low = 0;
        }

        public void AssignLeftShift(int count) {
            if (count >= 256) {
                High = 0;
                Low = 0;
                return;
            }
            if (count >= 128) {
                AssignLeftShift128();
                count -= 128;
            }
            if (count > 0) {
                High = (High << count) | (Low >> (128 - count));
                Low <<= count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            if (count >= 256) {
                High = 0;
                Low = 0;
                return;
            }
            if (count >= 128) {
                AssignRightShiftHalf();
                count -= 128;
            }
            if (count > 0) {
                Low = (Low >> count) | (High << (128 - count));
                High >>= count;
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

        public readonly BigInteger128 ExtractHigh128(int skipCount) {
            if (skipCount >= 256) {
                return new BigInteger128();
            }
            if (skipCount >= 128) {
                return BiLow << (skipCount - 128);
            }

            if (skipCount >= 64) {
                // skipCount -= 64; // no need to do this, shift size is masked by 0x3f
                if (skipCount == 64) {
                    return new BigInteger128(
                        UInt64[1],
                        UInt64[2]
                    );
                }

                return new BigInteger128(
                    (UInt64[1] << skipCount) | (UInt64[0] >> (64 - skipCount)),
                    (UInt64[2] << skipCount) | (UInt64[1] >> (64 - skipCount))
                );
            }

            if (skipCount == 0) {
                return BiHigh;
            }

            return new BigInteger128(
                (UInt64[2] << skipCount) | (UInt64[1] >> (64 - skipCount)),
                (UInt64[3] << skipCount) | (UInt64[2] >> (64 - skipCount))
            );
        }

        public readonly ulong ExtractHigh64(int skipCount) {
            //todo:
            var res = Clone();
            res.AssignLeftShift(skipCount);
            return res.HighUInt64;
        }
    }
}