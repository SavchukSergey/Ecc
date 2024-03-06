using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe struct BigInteger256 {

        public const int BITS_SIZE = 256;
        public const int BYTES_SIZE = BITS_SIZE / 8;
        private const int ITEM_BITS_SIZE = 32;
        internal const int ITEMS_SIZE = BITS_SIZE / ITEM_BITS_SIZE;

        internal fixed uint Data[ITEMS_SIZE];

        public BigInteger256() {
            for (var i = 0; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public BigInteger256(uint value) {
            Data[0] = value;
            for (var i = 1; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public BigInteger256(ulong value) {
            Data[0] = (uint)value;
            Data[1] = (uint)(value >> 32);
            for (var i = 2; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public BigInteger256(in BigInteger128 value) {
            ZeroExtendFrom(value);
        }

        [Obsolete]
        public BigInteger256(in BigInteger value) {
            var data = value.ToByteArray(isBigEndian: false);
            var si = 0;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                var bt0 = si < data.Length ? data[si++] : 0;
                var bt1 = si < data.Length ? data[si++] : 0;
                var bt2 = si < data.Length ? data[si++] : 0;
                var bt3 = si < data.Length ? data[si++] : 0;
                Data[i] = (uint)(bt3 << 24 | bt2 << 16 | bt1 << 8 | bt0);
            }
        }

        public BigInteger256(Span<byte> data) {
            var si = 0;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                var bt0 = si < data.Length ? data[si++] : 0;
                var bt1 = si < data.Length ? data[si++] : 0;
                var bt2 = si < data.Length ? data[si++] : 0;
                var bt3 = si < data.Length ? data[si++] : 0;
                Data[i] = (uint)(bt3 << 24 | bt2 << 16 | bt1 << 8 | bt0);
            }
        }

        public readonly byte GetByte(int index) {
            var btIndex = index >> 2;
            return (byte)(Data[btIndex] >> (8 * (index & 0x03)));
        }

        public readonly uint GetItem(int index) {
            return Data[index];
        }

        public readonly bool IsZero {
            get {
                for (var i = 0; i < ITEMS_SIZE; i++) {
                    if (Data[i] != 0) {
                        return false;
                    }
                }
                return true;
            }
        }

        public readonly bool IsEven {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return ((int)Data[0] & 0x1) == 0;
            }
        }

        public void Clear() {
            for (var i = 0; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public void AssignModAdd(in BigInteger256 other, in BigInteger256 modulus) {
            bool carry = false;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                ulong acc = Data[i];
                acc += other.Data[i];
                acc += carry ? 1ul : 0ul;
                Data[i] = (uint)acc;
                carry = acc > uint.MaxValue;
            }
            if (carry || this >= modulus) {
                this.AssignSub(modulus);
            }
        }

        public bool AssignAdd(in BigInteger256 other) {
            bool carry = false;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                ulong acc = Data[i];
                acc += other.Data[i];
                acc += carry ? 1ul : 0ul;
                Data[i] = (uint)acc;
                carry = acc > uint.MaxValue;
            }
            return carry;
        }

        public bool AssignSub(in BigInteger256 other) {
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

        public uint AssignShiftLeft() {
            uint carry = 0;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                var sum = (ulong)carry;
                sum += Data[i];
                sum += Data[i];
                Data[i] = (uint)sum;
                carry = (uint)(sum >> 32);
            }
            return carry;
        }

        public readonly BigInteger256 ModMul(in BigInteger256 other, in BigInteger256 modulus) {
            return new BigInteger256((this.ToNative() * other.ToNative()).ModAbs(modulus.ToNative()));
        }

        public void ZeroExtendFrom(in BigInteger128 source) {
            for (var i = 0; i < BigInteger128.ITEMS_SIZE; i++) {
                Data[i] = source.Data[i];
            }
            for (var i = BigInteger128.ITEMS_SIZE; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public readonly int Compare(in BigInteger256 other) {
            for (var i = ITEMS_SIZE - 1; i >= 0; i--) {
                var leftBt = Data[i];
                var rightBt = other.Data[i];
                if (leftBt > rightBt) {
                    return 1;
                }
                if (leftBt < rightBt) {
                    return -1;
                }
            }
            return 0;
        }

        public static int Compare(in BigInteger256 left, in BigInteger256 right) {
            for (var i = ITEMS_SIZE - 1; i >= 0; i--) {
                var leftBt = left.Data[i];
                var rightBt = right.Data[i];
                if (leftBt > rightBt) {
                    return 1;
                }
                if (leftBt < rightBt) {
                    return -1;
                }
            }
            return 0;
        }

        public static bool Equals(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) == 0;
        }

        [Obsolete]
        public readonly BigInteger ToNative() {
            var array = new byte[BYTES_SIZE];
            var ai = 0;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                var bt = Data[i];
                array[ai++] = (byte)(bt >> 0);
                array[ai++] = (byte)(bt >> 8);
                array[ai++] = (byte)(bt >> 16);
                array[ai++] = (byte)(bt >> 24);
            }
            return new BigInteger(array, isUnsigned: true, isBigEndian: false);
        }

        public static BigInteger256 operator /(BigInteger256 left, BigInteger256 right) {
            return new BigInteger256(left.ToNative() / right.ToNative());
        }

        public static BigInteger256 operator *(BigInteger256 left, BigInteger256 right) {
            return new BigInteger256(left.ToNative() * right.ToNative());
        }

        public static BigInteger256 operator %(BigInteger256 left, BigInteger256 right) {
            return new BigInteger256(left.ToNative() % right.ToNative());
        }

        public static BigInteger256 operator +(BigInteger256 left, BigInteger256 right) {
            var res = new BigInteger256();
            bool carry = false;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                ulong acc = left.Data[i];
                acc += right.Data[i];
                acc += carry ? 1ul : 0ul;
                res.Data[i] = (uint)acc;
                carry = acc > uint.MaxValue;
            }
            return res;
        }

        public static BigInteger256 operator -(in BigInteger256 left, in BigInteger256 right) {
            var res = new BigInteger256();
            bool carry = false;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                ulong acc = left.Data[i];
                acc -= right.Data[i];
                acc -= carry ? 1ul : 0ul;
                res.Data[i] = (uint)acc;
                carry = acc > uint.MaxValue;
            }
            return res;
        }

        public static bool operator <(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) < 0;
        }

        public static bool operator >(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) > 0;
        }

        public static bool operator <=(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator >=(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator ==(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) != 0;
        }

        public void ReadFromHex(ReadOnlySpan<char> str) {
            if (str.Length > BYTES_SIZE * 2) {
                throw new ArgumentException($"Expected hex string with {BYTES_SIZE * 2} characters");
            }
            var ptr = 0;
            var charPtr = str.Length - 1;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                uint part = 0;
                for (var j = 0; j < 32 && charPtr >= 0; j += 4) {
                    var hd = StringUtils.GetHexDigit(str[charPtr]);
                    part |= ((uint)hd << j);
                    charPtr--;
                }
                Data[ptr++] = part;
            }
        }

        public readonly bool TryWrite(Span<byte> buffer) {
            if (buffer.Length < BYTES_SIZE) {
                return false;
            }
            var ptr = 0;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                var val = Data[i];
                buffer[ptr++] = (byte)(val & 0xff);
                val >>= 8;
                buffer[ptr++] = (byte)(val & 0xff);
                val >>= 8;
                buffer[ptr++] = (byte)(val & 0xff);
                val >>= 8;
                buffer[ptr++] = (byte)val;
            }
            return true;
        }

    }
}
