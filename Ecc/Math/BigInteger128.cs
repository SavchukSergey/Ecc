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

        [FieldOffset(0)]
        internal fixed uint Data[ITEMS_SIZE]; //todo: review usages

        [FieldOffset(0)]
        internal fixed byte Bytes[BYTES_SIZE];

        [FieldOffset(0)]
        public UInt128 UInt128;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly byte GetByte(int index) {
            return Bytes[index];
        }

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

        public readonly BigInteger ToNative() {
            Span<byte> array = stackalloc byte[BYTES_SIZE];
            for (var i = 0; i < BYTES_SIZE; i++) {
                var bt = Bytes[i];
                array[i] = bt;
            }
            return new BigInteger(array, isUnsigned: true, isBigEndian: false);
        }
    }
}