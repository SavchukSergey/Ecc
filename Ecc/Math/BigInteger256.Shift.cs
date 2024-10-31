using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignLeftShift8() {
            HighUInt128 = (HighUInt128 << 8) + BiLow128.HighByte;
            LowUInt128 <<= 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignLeftShift16() {
            HighUInt128 = (HighUInt128 << 16) + BiLow128.HighUInt16;
            LowUInt128 <<= 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignLeftShift32() {
            HighUInt128 = (HighUInt128 << 32) + BiLow128.HighUInt32;
            LowUInt128 <<= 32;
        }

        public void AssignLeftShift64() {
            UInt64[3] = UInt64[2];
            UInt64[2] = UInt64[1];
            UInt64[1] = UInt64[0];
            UInt64[0] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignLeftShift128() {
            HighUInt128 = LowUInt128;
            LowUInt128 = 0;
        }

        public void AssignLeftShift(int count) {
            if (count >= 256) {
                HighUInt128 = 0;
                LowUInt128 = 0;
                return;
            }
            if (count >= 128) {
                AssignLeftShift128();
                count -= 128;
            }
            if (count > 0) {
                HighUInt128 = (HighUInt128 << count) | (LowUInt128 >> (128 - count));
                LowUInt128 <<= count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignRightShift128() {
            LowUInt128 = HighUInt128;
            HighUInt128 = 0;
        }

        public void AssignRightShift64() {
            UInt64[0] = UInt64[1];
            UInt64[1] = UInt64[2];
            UInt64[2] = UInt64[3];
            UInt64[3] = 0;
        }

        public void AssignRightShift(int count) {
            if (count >= 256) {
                HighUInt128 = 0;
                LowUInt128 = 0;
                return;
            }
            if (count >= 128) {
                AssignRightShift128();
                count -= 128;
            }
            if (count > 0) {
                LowUInt128 = (LowUInt128 >> count) | (HighUInt128 << (128 - count));
                HighUInt128 >>= count;
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
                return BiLow128 << (skipCount - 128);
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
                return BiHigh128;
            }

            return new BigInteger128(
                (UInt64[2] << skipCount) | (UInt64[1] >> (64 - skipCount)),
                (UInt64[3] << skipCount) | (UInt64[2] >> (64 - skipCount))
            );
        }

        public readonly ulong ExtractHigh64(int skipCount) {
            if (skipCount >= 128) {
                return BiLow128.ExtractHigh64(skipCount - 128);
            }
            if (skipCount >= 64) {
                //todo:
                var res = Clone();
                res.AssignLeftShift(skipCount);
                return res.HighUInt64;
            }
              if (skipCount == 0) {
                return HighUInt64;
            }

            return (UInt64[3] << skipCount) | (UInt64[2] >> (64 - skipCount));
        }
    }
}