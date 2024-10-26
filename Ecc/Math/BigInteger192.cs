using System;
using System.Numerics;
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
        public uint LowUInt32;

        [FieldOffset(0)]
        internal fixed byte Bytes[BYTES_SIZE];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger192(ulong low, ulong middle, ulong high) {
            LowUInt64 = low;
            MiddleUInt64 = middle;
            HighUInt64 = high;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger192(ulong low) {
            LowUInt64 = low;
            MiddleUInt64 = 0;
            HighUInt64 = 0;
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

        public readonly int LeadingZeroCount() {
            var val = 0;
            for (var i = UINT64_SIZE - 1; i >= 0; i--) {
                var cnt = BitOperations.LeadingZeroCount(UInt64[i]);
                if (cnt < 64) {
                    return val + cnt;
                }
                val += 64;
            }
            return val;
        }

        public readonly BigInteger192 Clone() {
            return new BigInteger192(LowUInt64, MiddleUInt64, HighUInt64);
        }

        public readonly BigInteger ToNative() {
            Span<byte> array = stackalloc byte[BYTES_SIZE];
            for (var i = 0; i < BYTES_SIZE; i++) {
                var bt = Bytes[i];
                array[i] = bt;
            }
            return new BigInteger(array, isUnsigned: true, isBigEndian: false);
        }

        public readonly bool IsZero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return UInt64[0] == 0 && UInt64[1] == 0 && UInt64[2] == 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignIncrement() {
            LowUInt128++;
            if (LowUInt128 == 0) {
                HighUInt64++;
            }
        }

        public static readonly BigInteger192 Zero = new();

    }
}
