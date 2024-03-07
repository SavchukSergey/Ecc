using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Ecc.Math {
    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public unsafe struct BigInteger512 {

        public const int BITS_SIZE = 512;
        public const int BYTES_SIZE = BITS_SIZE / 8;
        private const int ITEM_BITS_SIZE = 32;
        internal const int ITEMS_SIZE = BITS_SIZE / ITEM_BITS_SIZE;

        [FieldOffset(0)]
        internal fixed uint Data[ITEMS_SIZE]; //todo: review usages

        [FieldOffset(0)]
        public BigInteger256 Low;

        [FieldOffset(32)]
        public BigInteger256 High;

        public BigInteger512() {
            for (var i = 0; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public BigInteger512(uint value) {
            Data[0] = value;
            for (var i = 1; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public BigInteger512(ulong value) {
            Data[0] = (uint)value;
            Data[1] = (uint)(value >> 32);
            for (var i = 2; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public BigInteger512(in BigInteger256 value) {
            Low = value;
            High = new BigInteger256(0);
        }

        public BigInteger512(BigInteger256 low, BigInteger256 high) {
            Low = low;
            High = high;
        }

        [Obsolete]
        public BigInteger512(in BigInteger value) {
            var data = value.ToByteArray(isBigEndian: false);
            var si = 0;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                var bt0 = si < data.Length ? data[si++] : 0;
                var bt1 = si < data.Length ? data[si++] : 0;
                var bt2 = si < data.Length ? data[si++] : 0;
                var bt3 = si < data.Length ? data[si++] : 0;
                Data[i] = (uint)((bt3 << 24) | (bt2 << 16) | (bt1 << 8) | bt0);
            }
        }

        public readonly byte GetByte(int index) {
            var btIndex = index >> 2;
            return (byte)(Data[btIndex] >> (8 * (index & 0x03)));
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

        public readonly bool IsZero {
            get {
                return Low.IsZero && High.IsZero;
            }
        }

        public void Clear() {
            for (var i = 0; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public bool AssignAdd(in BigInteger512 other) {
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

        public bool AssignAdd(in BigInteger256 other) {
            bool carry = false;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                ulong acc = Data[i];
                acc += i < BigInteger256.ITEMS_SIZE ? other.Data[i] : 0;
                acc += carry ? 1ul : 0ul;
                Data[i] = (uint)acc;
                carry = acc > uint.MaxValue;
            }
            return carry;
        }

        public void AssignLeftShiftQuarter() {
            Data[15] = Data[11];
            Data[14] = Data[10];
            Data[13] = Data[9];
            Data[12] = Data[8];
            Data[11] = Data[7];
            Data[10] = Data[6];
            Data[9] = Data[5];
            Data[8] = Data[4];
            Data[7] = Data[3];
            Data[6] = Data[2];
            Data[5] = Data[1];
            Data[4] = Data[0];
            Data[3] = 0;
            Data[2] = 0;
            Data[1] = 0;
            Data[0] = 0;
        }

        public void AssignLeftShiftHalf() {
            Data[15] = Data[7];
            Data[14] = Data[6];
            Data[13] = Data[5];
            Data[12] = Data[4];
            Data[11] = Data[3];
            Data[10] = Data[2];
            Data[9] = Data[1];
            Data[8] = Data[0];
            Data[7] = 0;
            Data[6] = 0;
            Data[5] = 0;
            Data[4] = 0;
            Data[3] = 0;
            Data[2] = 0;
            Data[1] = 0;
            Data[0] = 0;
        }

        public void AssignLeftShift32() {
            Data[15] = Data[14];
            Data[14] = Data[13];
            Data[13] = Data[12];
            Data[12] = Data[11];
            Data[11] = Data[10];
            Data[10] = Data[9];
            Data[9] = Data[8];
            Data[8] = Data[7];
            Data[7] = Data[6];
            Data[6] = Data[5];
            Data[5] = Data[4];
            Data[4] = Data[3];
            Data[3] = Data[2];
            Data[2] = Data[1];
            Data[1] = Data[0];
            Data[0] = 0;
        }

        public bool Sub(in BigInteger512 other) {
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

        public bool AssignLeftShift() {
            return AssignAdd(this);
        }

        public uint ShiftLeft() {
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

        public void ZeroExtendFrom(in BigInteger256 source) {
            for (var i = 0; i < BigInteger256.ITEMS_SIZE; i++) {
                Data[i] = source.Data[i];
            }
            for (var i = BigInteger256.ITEMS_SIZE; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public readonly int Compare(in BigInteger512 other) {
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

        public static BigInteger512 operator +(BigInteger512 left, BigInteger512 right) {
            var res = new BigInteger512();
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

        public static BigInteger512 operator %(BigInteger512 left, BigInteger512 right) {
            return new BigInteger512(left.ToNative() % right.ToNative());
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

        public readonly string ToHexUnsigned(int length = 64) {
            var sbLength = (int)length * 2;
            var sb = new StringBuilder(sbLength, sbLength);
            var dataLength = BYTES_SIZE;
            const string hex = "0123456789abcdef";
            for (var i = length - 1; i >= 0; i--) {
                if (i < dataLength) {
                    var ch = GetByte(i);
                    sb.Append(hex[ch >> 4]);
                    sb.Append(hex[ch & 0x0f]);
                } else {
                    sb.Append("00");
                }
            }
            return sb.ToString();
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
