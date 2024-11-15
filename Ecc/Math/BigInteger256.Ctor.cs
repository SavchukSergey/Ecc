using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public BigInteger256() {
            LowUInt128 = 0;
            HighUInt128 = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger256(uint value) {
            LowUInt128 = value;
            HighUInt128 = 0;
        }

        public BigInteger256(uint b0, uint b1) {
            UInt32[0] = b0;
            UInt32[1] = b1;
            UInt64[1] = 0;
            HighUInt128 = 0;
        }

        public BigInteger256(uint b0, uint b1, uint b2, uint b3) {
            UInt32[0] = b0;
            UInt32[1] = b1;
            UInt32[2] = b2;
            UInt32[3] = b3;
            HighUInt128 = 0;
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger256(ulong value) {
            LowUInt128 = value;
            HighUInt128 = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger256(UInt128 low) {
            LowUInt128 = low;
            HighUInt128 = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger256(in BigInteger128 low) {
            BiLow128 = low;
            HighUInt128 = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger256(in BigInteger192 low) {
            BiLow192 = low;
            HighUInt64 = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger256(UInt128 low, UInt128 high) {
            LowUInt128 = low;
            HighUInt128 = high;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger256(in BigInteger256 other) {
            BiLow128 = other.BiLow128;
            BiHigh128 = other.BiHigh128;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger256(in BigInteger128 low, in BigInteger128 high) {
            BiLow128 = low;
            BiHigh128 = high;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger256(ulong low, in BigInteger192 high) {
            LowUInt64 = low;
            BiHigh192 = high;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger256(ulong low, in BigInteger128 high) {
            LowUInt64 = low;
            BiMiddle128 = high;
            HighUInt64 = 0;
        }

        public BigInteger256(in BigInteger value) {
            Span<byte> bytes = stackalloc byte[value.GetByteCount()];
            value.TryWriteBytes(bytes, out var bytesWritten, isUnsigned: true, isBigEndian: false);
            for (var i = 0; i < bytesWritten; i++) {
                Bytes[i] = i < bytes.Length ? bytes[i] : (byte)0;
            }
            for (var i = bytesWritten; i < BYTES_SIZE; i++) {
                Bytes[i] = 0;
            }
        }

        public BigInteger256(ReadOnlySpan<byte> data) {
            for (var i = 0; i < BYTES_SIZE; i++) {
                Bytes[i] = i < data.Length ? data[i] : (byte)0;
            }
        }

        public BigInteger256(ReadOnlySpan<byte> data, bool bigEndian = false) {
            var srcIndex = bigEndian ? BYTES_SIZE - 1 : 0;
            var srcDelta = bigEndian ? -1 : 1;
            for (var destIndex = 0; destIndex < BYTES_SIZE; destIndex++) {
                Bytes[destIndex] = srcIndex < data.Length ? data[srcIndex] : (byte)0;
                srcIndex += srcDelta;
            }
        }

        public static readonly BigInteger256 Zero = new();
    }
}