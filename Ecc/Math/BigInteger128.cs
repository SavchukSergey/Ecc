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
        private const int ITEM_BITS_SIZE = 32;
        internal const int ITEMS_SIZE = BITS_SIZE / ITEM_BITS_SIZE;
        internal const int UINT64_SIZE = BITS_SIZE / 64;
        internal const int UINT32_SIZE = BITS_SIZE / 32;
        internal const int UINT16_SIZE = BITS_SIZE / 16;

        [FieldOffset(0)]
        internal fixed uint Data[ITEMS_SIZE]; //todo: review usages

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

        [FieldOffset(8)]
        internal ulong HighUInt64;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly byte GetByte(int index) {
            return Bytes[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BigInteger128(UInt128 value) {
            UInt128 = value;
        }

        public BigInteger128(in BigInteger value) {
            var data = value.ToByteArray(isBigEndian: false);
            for (var i = 0; i < BYTES_SIZE; i++) {
                Bytes[i] = i < data.Length ? data[i] : (byte)0;
            }
        }

        public static BigInteger128 DivRem(in BigInteger128 dividend, in BigInteger128 divisor, out BigInteger128 remainder) {
            var res = new BigInteger128(dividend.UInt128 / divisor.UInt128);
            remainder = new BigInteger128(dividend.UInt128 % divisor.UInt128);
            return res;
        }

        public static BigInteger128 DivRem64(in BigInteger128 dividend, ulong divisor, out BigInteger128 remainder) {
            var res = new BigInteger128(dividend.UInt128 / divisor);
            remainder = new BigInteger128(dividend.UInt128 % divisor);
            return res;
        }

        public static BigInteger128 DivRem64(in UInt128 dividend, ulong divisor, out BigInteger128 remainder) {
            var res = new BigInteger128(dividend / divisor);
            remainder = new BigInteger128(dividend % divisor);
            return res;
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
        public void AssignLeftShift(int count) {
            UInt128 <<= count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignSub(in BigInteger128 other) {
            UInt128 -= other.UInt128;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignAdd(in BigInteger128 other) {
            UInt128 += other.UInt128;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignAdd(in UInt128 other) {
            UInt128 += other;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AssignAddHigh(ulong other) {
            High += other;
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

    }
}