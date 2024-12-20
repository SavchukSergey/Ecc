using System;
using System.Numerics;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public BigInteger512() {
            for (var i = 0; i < UINT64_SIZE; i++) {
                UInt64[i] = 0;
            }
        }

        public BigInteger512(uint value) {
            UInt32[0] = value;
            for (var i = 1; i < UINT32_SIZE; i++) {
                UInt32[i] = 0;
            }
        }

        public BigInteger512(ulong value) {
            UInt64[0] = value;
            for (var i = 1; i < UINT64_SIZE; i++) {
                UInt64[i] = 0;
            }
        }

        public BigInteger512(ulong b0, ulong b1, ulong b2, ulong b3, ulong b4, ulong b5, ulong b6, ulong b7) {
            UInt64[0] = b0;
            UInt64[1] = b1;
            UInt64[2] = b2;
            UInt64[3] = b3;
            UInt64[4] = b4;
            UInt64[5] = b5;
            UInt64[6] = b6;
            UInt64[7] = b7;
        }

        public BigInteger512(in BigInteger128 value) {
            BiLow128 = value;
            BiHigh384.Clear();
        }

        public BigInteger512(in BigInteger256 value) {
            BiLow256 = value;
            BiHigh256.Clear();
        }

        public BigInteger512(in BigInteger384 value) {
            BiLow384 = value;
            BiHigh128 = BigInteger128.Zero;
        }

        public BigInteger512(in BigInteger256 low, in BigInteger256 high) {
            BiLow256 = low;
            BiHigh256 = high;
        }

        public BigInteger512(in BigInteger512 other) {
            BiLow256 = other.BiLow256;
            BiHigh256 = other.BiHigh256;
        }

        public BigInteger512(in BigInteger value) {
            Span<byte> bytes = stackalloc byte[value.GetByteCount()];
            value.TryWriteBytes(bytes, out var bytesWritten, isUnsigned: true, isBigEndian: false);
            for (var i = 0; i < bytesWritten; i++) {
                Bytes[i] = i < bytes.Length ? bytes[i] : (byte)0;
            }
            for (var i = bytesWritten; i < BYTES_SIZE; i++) {
                Bytes[i] = 0;
            }
        }

    }
}