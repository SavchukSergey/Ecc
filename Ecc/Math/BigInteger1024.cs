using System.Runtime.InteropServices;
using System.Text;

namespace Ecc.Math {
    [StructLayout(LayoutKind.Explicit, Size = 128)]
    public unsafe partial struct BigInteger1024 {

        public const int BITS_SIZE = 1024;
        public const int BYTES_SIZE = BITS_SIZE / 8;
        private const int ITEM_BITS_SIZE = 32;
        internal const int ITEMS_SIZE = BITS_SIZE / ITEM_BITS_SIZE;
        internal const int UINT64_SIZE = BITS_SIZE / 64;

        [FieldOffset(0)]
        internal fixed uint Data[ITEMS_SIZE]; //todo: review usages

        [FieldOffset(0)]
        public BigInteger512 Low;

        [FieldOffset(64)]
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

        public static BigInteger1024 operator +(in BigInteger1024 left, in BigInteger1024 right) {
            var res = new BigInteger1024(left);
            res.AssignAdd(right);
            return res;
        }

        public bool AssignAdd(in BigInteger1024 other) {
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

        public static BigInteger1024 operator -(in BigInteger1024 left, in BigInteger1024 right) {
            var res = new BigInteger1024(left);
            res.AssignSub(right);
            return res;
        }

        public readonly string ToHexUnsigned(int length = 256) {
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
    }
}
