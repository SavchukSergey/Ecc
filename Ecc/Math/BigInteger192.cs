using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ecc.Math {
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    [SkipLocalsInit]
    public unsafe partial struct BigInteger192 {

        public const int BITS_SIZE = 192;
        public const int BYTES_SIZE = BITS_SIZE / 8;
        internal const int UINT64_SIZE = BITS_SIZE / 64;
        internal const int UINT32_SIZE = BITS_SIZE / 32;

        public const int HEX_SIZE = BYTES_SIZE * 2;

        [FieldOffset(0)]
        internal fixed ulong UInt64[UINT64_SIZE];
        [FieldOffset(0)]
        internal fixed uint UInt32[UINT32_SIZE];

        [FieldOffset(0)]
        public UInt128 LowUInt128;

        [FieldOffset(8)]
        public UInt128 HighUInt128;

        [FieldOffset(0)]
        public BigInteger128 BiLow128;

        [FieldOffset(8)]
        public BigInteger128 BiHigh128;

        [FieldOffset(0)]
        public ulong LowUInt64;

        [FieldOffset(8)]
        public ulong MiddleUInt64;

        [FieldOffset(16)]
        public ulong HighUInt64;

        [FieldOffset(0)]
        internal fixed byte Bytes[BYTES_SIZE];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger192(ulong low, ulong middle, ulong high) {
            LowUInt64 = low;
            MiddleUInt64 = middle;
            HighUInt64 = high;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger192(in BigInteger128 low) {
            BiLow128 = low;
            HighUInt64 = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger192(ulong low, in BigInteger128 high) {
            LowUInt64 = low;
            BiHigh128 = high;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger192(in BigInteger192 other) {
            LowUInt128 = other.LowUInt128;
            HighUInt64 = other.HighUInt64;
        }

       [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly byte GetByte(int index) {
            return Bytes[index];
        }

        public static readonly BigInteger192 Zero = new();

    }
}
