using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ecc.Math {
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    [SkipLocalsInit]
    public unsafe partial struct BigInteger384 {

        public const int BITS_SIZE = 384;
        public const int BYTES_SIZE = BITS_SIZE / 8;
        internal const int UINT64_SIZE = BITS_SIZE / 64;
        internal const int UINT32_SIZE = BITS_SIZE / 32;

        public const int HEX_SIZE = BYTES_SIZE * 2;

        [FieldOffset(0)]
        internal fixed ulong UInt64[UINT64_SIZE];

        [FieldOffset(0)]
        public UInt128 LowUInt128;

        [FieldOffset(16)]
        public UInt128 MiddleUInt128;

        [FieldOffset(32)]
        public UInt128 HighUInt128;

        [FieldOffset(0)]
        public ulong LowUInt64;

        [FieldOffset(40)]
        public ulong HighUInt64;

        [FieldOffset(0)]
        public BigInteger192 BiLow192;

        [FieldOffset(0)]
        public BigInteger256 BiLow256;

        [FieldOffset(16)]
        public BigInteger256 BiHigh256;

        [FieldOffset(24)]
        public BigInteger192 BiHigh192;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger384() {
            LowUInt128 = 0;
            MiddleUInt128 = 0;
            HighUInt128 = 0;
        }

        public BigInteger384(ulong low, BigInteger192 m0) {
            UInt64[0] = low;
            UInt64[1] = m0.UInt64[0];
            UInt64[2] = m0.UInt64[1];
            UInt64[3] = m0.UInt64[2];
            HighUInt128 = 0;
        }

        public BigInteger384(in BigInteger192 low) {
            BiLow192 = low;
            BiHigh192 = BigInteger192.Zero;
        }

        public BigInteger384(in BigInteger256 low) {
            BiLow256 = low;
            HighUInt64 = 0;
        }

        public BigInteger384(ulong low, in BigInteger256 high) {
            LowUInt64 = low;
            BiHigh256 = high;
        }

        public BigInteger384(in BigInteger384 other) {
            BiLow192 = other.BiLow192;
            BiHigh192 = other.BiHigh192;
        }
        public static readonly BigInteger384 Zero = new();

    }
}
