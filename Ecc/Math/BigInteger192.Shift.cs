namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        public void AssignLeftShift(int count) {
            if (count >= 192) {
                BiHigh128 = BigInteger128.Zero;
                LowUInt64 = 0;
                return;
            }
            if (count >= 128) {
                HighUInt64 = LowUInt64 << (count - 128);
                LowUInt128 = 0;
            }
            if (count >= 64) {
                count -= 64;
                UInt64[2] = (UInt64[1] << count) | (UInt64[0] >> (64 - count));
                UInt64[1] = UInt64[0] << count;
                UInt64[0] = 0;;
            }
            if (count > 0) {
                UInt64[2] = (UInt64[2] << count) | (UInt64[1] >> (64 - count));
                UInt64[1] = (UInt64[1] << count) | (UInt64[0] >> (64 - count));
                UInt64[0] <<= count;
            }
        }

        public readonly BigInteger128 ExtractHigh128(int skipCount) {
            if (skipCount >= 192) {
                return new BigInteger128();
            }
            if (skipCount >= 64) {
                return BiLow128 << (skipCount - 64);
            }

            if (skipCount == 0) {
                return BiHigh128;
            }

            return new BigInteger128(
                (MiddleUInt64 << skipCount) | (LowUInt64 >> (64 - skipCount)),
                (HighUInt64 << skipCount) | (MiddleUInt64 >> (64 - skipCount))
            );
        }

        public readonly ulong ExtractHigh64(int skipCount) {
            if (skipCount >= 192) {
                return 0;
            }
            if (skipCount >= 128) {
                return LowUInt64 << (skipCount - 128);
            }
            if (skipCount >= 64) {
                return (ulong)((LowUInt128 << skipCount) >> 64);
            }
            return (ulong)((HighUInt128 << skipCount) >> 64);
        }

    }
}