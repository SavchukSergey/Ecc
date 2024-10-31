using System.Numerics;
using System;
using System.Runtime.InteropServices;

namespace Ecc.Math {
    [StructLayout(LayoutKind.Explicit, Size = 128)]
    public unsafe partial struct BigInteger1024 {

        public const int BITS_SIZE = 1024;
        public const int BYTES_SIZE = BITS_SIZE / 8;
        private const int ITEM_BITS_SIZE = 32;
        internal const int ITEMS_SIZE = BITS_SIZE / ITEM_BITS_SIZE;
        internal const int UINT64_SIZE = BITS_SIZE / 64;

        public const int HEX_SIZE = BYTES_SIZE * 2;

        [FieldOffset(0)]
        internal fixed byte Bytes[BYTES_SIZE];

        [FieldOffset(0)]
        internal fixed uint Data[ITEMS_SIZE]; //todo: review usages

        [FieldOffset(0)]
        public BigInteger512 BiLow512;

        [FieldOffset(0)]
        [Obsolete]
        public BigInteger512 Low;

        [FieldOffset(64)]
        [Obsolete]
        public BigInteger512 High;

        [FieldOffset(32)]
        public BigInteger512 Middle; // for 512.512 fixed point arithmetics

        [FieldOffset(0)]
        internal fixed ulong UInt64[UINT64_SIZE];

        public BigInteger1024(in BigInteger512 low) {
            Low = low;
            High = new BigInteger512(0);
        }

        public BigInteger1024(in BigInteger512 low, in BigInteger512 high) {
            Low = low;
            High = high;
        }

        public BigInteger1024(in BigInteger1024 other) {
            Low = other.Low;
            High = other.High;
        }

        public readonly byte GetByte(int index) {
            var btIndex = index >> 2;
            return (byte)(Data[btIndex] >> (8 * (index & 0x03)));
        }

        public readonly bool IsOne {
            get {
                return Low.IsOne && High.IsZero;
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

        public static BigInteger1024 operator +(in BigInteger1024 left, in BigInteger1024 right) {
            var res = new BigInteger1024(left);
            res.AssignAdd(right);
            return res;
        }

        public bool AssignAdd(in BigInteger1024 other) {
            byte carry = 0;
            for (var i = 0; i < UINT64_SIZE; i++) {
                UInt128 acc = UInt64[i];
                acc += other.UInt64[i];
                acc += carry;
                UInt64[i] = (ulong)acc;
                carry = (byte)(acc >> 64);
            }
            return carry > 0;
        }

        public bool AssignSub(in BigInteger1024 other) {
            bool carry = false;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                ulong acc = Data[i];
                acc -= other.Data[i];
                acc -= carry ? 1ul : 0ul;
                Data[i] = (uint)acc;
                carry = acc > uint.MaxValue;
            }
            return carry;
        }

        public void AssignSub(in BigInteger512 other) {
            bool carry = false;
            for (var i = 0; i < UINT64_SIZE / 2; i++) {
                UInt128 acc = UInt64[i];
                acc -= other.UInt64[i];
                acc -= carry ? 1u : 0u;
                UInt64[i] = (ulong)acc;
                carry = acc > ulong.MaxValue; //todo: use shift to avoid branching
            }
            if (carry) {
                High.AssignDecrement();
            }
        }

        public static BigInteger1024 operator -(in BigInteger1024 left, in BigInteger1024 right) {
            var res = new BigInteger1024(left);
            res.AssignSub(right);
            return res;
        }

        public static BigInteger1024 operator -(in BigInteger1024 left, in BigInteger512 right) {
            var res = new BigInteger1024(left);
            res.AssignSub(right);
            return res;
        }

        public readonly string ToHexUnsigned() {
            Span<char> chars = stackalloc char[HEX_SIZE];
            TryWriteHex(chars);
            return new string(chars);
        }

        public readonly bool TryWriteHex(Span<char> output) {
            if (output.Length < HEX_SIZE) {
                return false;
            }
            const string hex = "0123456789abcdef";
            var ptr = 0;
            for (var i = BYTES_SIZE - 1; i >= 0; i--) {
                var ch = GetByte(i);
                output[ptr++] = hex[ch >> 4];
                output[ptr++] = hex[ch & 0x0f];
            }
            return true;
        }
    }
}
