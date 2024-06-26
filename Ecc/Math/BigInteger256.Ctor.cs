using System;
using System.Numerics;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public BigInteger256() {
            BiLow.Clear();
            BiHigh.Clear();
        }

        public BigInteger256(uint value) {
            Low = value;
            BiHigh.Clear();
        }

        public BigInteger256(uint b0, uint b1) {
            UInt32[0] = b0;
            UInt32[1] = b1;
            UInt64[1] = 0;
            BiHigh.Clear();
        }

        public BigInteger256(uint b0, uint b1, uint b2, uint b3) {
            UInt32[0] = b0;
            UInt32[1] = b1;
            UInt32[2] = b2;
            UInt32[3] = b3;
            BiHigh.Clear();
        }

        public BigInteger256(ulong b0, ulong b1, ulong b2, ulong b3) {
            UInt64[0] = b0;
            UInt64[1] = b1;
            UInt64[2] = b2;
            UInt64[3] = b3;
        }

        public BigInteger256(uint b0, uint b1, uint b2, uint b3, uint b4, uint b5, uint b6, uint b7) {
            UInt32[0] = b0;
            UInt32[1] = b1;
            UInt32[2] = b2;
            UInt32[3] = b3;
            UInt32[4] = b4;
            UInt32[5] = b5;
            UInt32[6] = b6;
            UInt32[7] = b7;
        }

        public BigInteger256(ulong value) {
            UInt64[0] = value;
            UInt64[1] = 0;
            UInt64[2] = 0;
            UInt64[3] = 0;
        }

        public BigInteger256(UInt128 low) {
            Low = low;
            BiHigh.Clear();
        }

        public BigInteger256(in BigInteger128 low) {
            BiLow = low;
            BiHigh.Clear();
        }

        public BigInteger256(UInt128 low, UInt128 high) {
            Low = low;
            High = high;
        }

        public BigInteger256(in BigInteger256 other) {
            Low = other.Low;
            High = other.High;
        }

        public BigInteger256(in BigInteger128 low, in BigInteger128 high) {
            BiLow = low;
            BiHigh = high;
        }

        public BigInteger256(in BigInteger value) {
            var data = value.ToByteArray(isBigEndian: false);
            for (var i = 0; i < BYTES_SIZE; i++) {
                Bytes[i] = i < data.Length ? data[i] : (byte)0;
            }
        }

        public BigInteger256(ReadOnlySpan<byte> data) {
            for (var i = 0; i < BYTES_SIZE; i++) {
                Bytes[i] = i < data.Length ? data[i] : (byte)0;
            }
        }
    }
}