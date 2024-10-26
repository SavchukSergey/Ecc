using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ecc.Math {
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    [SkipLocalsInit]
    public unsafe partial struct BigInteger128 {

        public const int BITS_SIZE = 128;
        public const int BYTES_SIZE = BITS_SIZE / 8;
        internal const int UINT64_SIZE = BITS_SIZE / 64;
        internal const int UINT32_SIZE = BITS_SIZE / 32;
        internal const int UINT16_SIZE = BITS_SIZE / 16;

        public const int HEX_SIZE = BYTES_SIZE * 2;

        [FieldOffset(0)]
        internal fixed byte Bytes[BYTES_SIZE];

        [FieldOffset(0)]
        internal fixed uint UInt32[UINT32_SIZE];

        [FieldOffset(0)]
        internal fixed ulong UInt64[UINT64_SIZE];

        [FieldOffset(0)]
        public ulong Low;

        [FieldOffset(8)]
        public ulong High;

        [FieldOffset(0)]
        public UInt128 UInt128;

        [FieldOffset(0)]
        internal uint LowUInt32;

        [FieldOffset(0)]
        internal ulong LowUInt64;

        [FieldOffset(8)]
        internal ulong HighUInt64;

        [FieldOffset(12)]
        internal byte HighUInt32;

        [FieldOffset(14)]
        internal byte HighUInt16;

        [FieldOffset(15)]
        internal byte HighByte;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly byte GetByte(int index) {
            return Bytes[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger128(UInt128 value) {
            UInt128 = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger128(ulong low, ulong high) {
            Low = low;
            High = high;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() {
            UInt128 = 0;
        }

        public BigInteger128(in BigInteger value) {
            Span<byte> bytes = stackalloc byte[value.GetByteCount()];
            value.TryWriteBytes(bytes, out var bytesWritten, isUnsigned: true, isBigEndian: false);
            for (var i = 0; i < bytesWritten; i++) {
                Bytes[i] = i < bytes.Length ? bytes[i] : (byte)0;
            }
            for (var i = bytesWritten; i < BYTES_SIZE; i++) {
                Bytes[i] = 0;
            }
        }

        public readonly BigInteger ToNative() {
            Span<byte> array = stackalloc byte[BYTES_SIZE];
            for (var i = 0; i < BYTES_SIZE; i++) {
                var bt = Bytes[i];
                array[i] = bt;
            }
            return new BigInteger(array, isUnsigned: true, isBigEndian: false);
        }

        public readonly BigInteger128 Clone() {
            return new BigInteger128(UInt128);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignIncrement() {
            UInt128++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignDecrement() {
            UInt128--;
        }

        public readonly bool IsZero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return UInt128 == 0;
            }
        }

        public readonly bool IsOne {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return UInt128 == 1;
            }
        }

        public static readonly BigInteger128 Zero = new();
    }
}