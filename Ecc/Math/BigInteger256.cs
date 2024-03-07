using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Ecc.Math {
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public unsafe struct BigInteger256 {

        public const int BITS_SIZE = 256;
        public const int BYTES_SIZE = BITS_SIZE / 8;
        private const int ITEM_BITS_SIZE = 32;
        internal const int ITEMS_SIZE = BITS_SIZE / ITEM_BITS_SIZE;

        [FieldOffset(0)]
        internal fixed uint Data[ITEMS_SIZE]; //todo: review usages

        [FieldOffset(0)]
        public UInt128 Low;

        [FieldOffset(16)]
        public UInt128 High;

        public BigInteger256() {
            Low = 0;
            High = 0;
        }

        public BigInteger256(uint value) {
            Data[0] = value;
            for (var i = 1; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public BigInteger256(uint b0, uint b1) {
            Data[0] = b0;
            Data[1] = b1;
            for (var i = 2; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public BigInteger256(uint b0, uint b1, uint b2, uint b3) {
            Data[0] = b0;
            Data[1] = b1;
            Data[2] = b2;
            Data[3] = b3;
            Data[4] = 0;
            Data[5] = 0;
            Data[6] = 0;
            Data[7] = 0;
        }

        public BigInteger256(uint b0, uint b1, uint b2, uint b3, uint b4, uint b5, uint b6, uint b7) {
            Data[0] = b0;
            Data[1] = b1;
            Data[2] = b2;
            Data[3] = b3;
            Data[4] = b4;
            Data[5] = b5;
            Data[6] = b6;
            Data[7] = b7;
        }

        public BigInteger256(ulong value) {
            Data[0] = (uint)value;
            Data[1] = (uint)(value >> 32);
            for (var i = 2; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public BigInteger256(UInt128 low) {
            Low = low;
            High = 0;
        }

        public BigInteger256(UInt128 low, UInt128 high) {
            Low = low;
            High = high;
        }

        public BigInteger256(in BigInteger256 other) {
            Low = other.Low;
            High = other.High;
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

        public readonly bool GetBit(int index) {
            var btIndex = index >> 5;
            return (Data[btIndex] & (1 << (index & 0x1f))) != 0;
        }

        public void SetBit(int index) {
            var btIndex = index >> 5;
            Data[btIndex] |= (uint)(1 << (index & 0x1f));
        }

        public readonly uint GetItem(int index) {
            return Data[index];
        }

        public readonly bool IsZero {
            get {
                return Low == 0 && High == 0;
            }
        }

        public readonly bool IsEven {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return ((int)Data[0] & 0x1) == 0;
            }
        }

        public readonly BigInteger256 Clone() {
            return new BigInteger256(Low, High);
        }

        public void Clear() {
            Low = 0;
            High = 0;
        }

        public void AssignModAdd(in BigInteger256 other, in BigInteger256 modulus) {
            ulong carry = 0;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                ulong acc = Data[i];
                acc += other.Data[i];
                acc += carry;
                Data[i] = (uint)acc;
                carry = acc >> 32;
            }
            if (carry > 0 || this >= modulus) {
                AssignSub(modulus);
            }
        }

        public void AssignModDouble(in BigInteger256 modulus) {
            AssignModAdd(this, modulus);
        }

        public bool AssignAdd(in BigInteger256 other) {
            ulong carry = 0;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                ulong acc = Data[i];
                acc += other.Data[i];
                acc += carry;
                Data[i] = (uint)acc;
                carry = acc >> 32;
            }
            return carry > 0;
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

        public void AssignLeftShiftQuarter() {
            Data[7] = Data[5];
            Data[6] = Data[4];
            Data[5] = Data[3];
            Data[4] = Data[2];
            Data[3] = Data[1];
            Data[2] = Data[0];
            Data[1] = 0;
            Data[0] = 0;
        }

        public void AssignLeftShiftHalf() {
            High = Low;
            Low = 0;
        }

        public readonly BigInteger256 ModAdd(in BigInteger256 other, in BigInteger256 modulus) {
            var res = new BigInteger256(this);
            var carry = res.AssignAdd(other);
            if (carry || res >= modulus) {
                res.AssignSub(modulus);
            }
            return res;
        }

        public readonly BigInteger256 ModSub(in BigInteger256 other, in BigInteger256 modulus) {
            var res = new BigInteger256(); //todo: unneccessary zeroing
            bool carry = false;
            for (var i = 0; i < ITEMS_SIZE; i++) {
                ulong acc = Data[i];
                acc -= other.Data[i];
                acc -= carry ? 1ul : 0ul;
                res.Data[i] = (uint)acc;
                carry = acc > uint.MaxValue;
            }
            if (carry || res >= modulus) {
                res.AssignAdd(modulus);
            }
            return res;
        }

        public readonly BigInteger256 ModTriple(in BigInteger256 modulus) {
            var res = Clone();
            res.AssignModAdd(this, modulus);
            res.AssignModAdd(this, modulus);
            return res;
        }

        // public readonly void ModMul(in BigInteger256 other, in BigInteger256 modulus, out BigInteger256 result) {
        //     result.Clear();
        //     var walker = Clone();
        //     for (var bit = 0; bit < BITS_SIZE; bit++) {
        //         if (other.GetBit(bit)) {
        //             result.AssignModAdd(walker, modulus);
        //         }
        //         walker.AssignModDouble(modulus);
        //     }
        //     //todo: above too slow
        // }

        // public readonly void ModMul(in BigInteger256 other, in BigInteger256 modulus, out BigInteger256 result) {
        //     result = new BigInteger256((this.ToNative() * other.ToNative()).ModAbs(modulus.ToNative()));
        // }

        public readonly BigInteger256 ModMul(in BigInteger256 other, in BigInteger256 modulus) {
            //todo: modular all the way
            return ((this * other) % new BigInteger512(modulus)).Low;
            // return ModMulBit(other, modulus);
        }

        public readonly BigInteger256 ModMulBit(in BigInteger256 other, in BigInteger256 modulus) {
            var result = new BigInteger256(0);
            var acc = new BigInteger256(this);
            for (var i = 0; i < BITS_SIZE; i++) {
                if (other.GetBit(i)) {
                    result.AssignModAdd(acc, modulus);
                }
                acc.AssignModDouble(modulus);
            }
            return result;
        }

        public readonly BigInteger256 ModPow(in BigInteger256 exp, in BigInteger256 modulus) {
            var acc = new BigInteger256(1);
            var walker = Clone();
            for (var bit = 0; bit < BITS_SIZE; bit++) {
                //todo: use even odd bits to use ModMul with out param
                if (exp.GetBit(bit)) {
                    acc = acc.ModMul(walker, modulus);
                }
                walker = walker.ModSquare(modulus);
            }
            return acc;
            //todo: above to slow?
        }

        public readonly BigInteger256 ModSquare(in BigInteger256 modulus) {
            return ModMul(this, modulus);
        }

        public readonly BigInteger256 ModCube(in BigInteger256 modulus) {
            return ModMul(ModMul(this, modulus), modulus);
        }

        public readonly BigInteger256 ModDouble(in BigInteger256 modulus) {
            return ModAdd(this, modulus);
        }

        public readonly BigInteger256 ModInverse(in BigInteger256 modulus) {
            return BigInteger256Ext.EuclidExtended(this, modulus).X % modulus;
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
            if (left.High < right.High) {
                return -1;
            }
            if (left.High > right.High) {
                return 1;
            }
            if (left.Low < right.Low) {
                return -1;
            }
            if (left.Low > right.Low) {
                return 1;
            }
            return 0;
        }

        public static bool Equals(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) == 0;
        }

        public readonly override bool Equals(object? other) {
            if (other is BigInteger256 val) {
                return Equals(this, val);
            }
            return false;
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

        public static BigInteger256 operator /(in BigInteger256 left, in BigInteger256 right) {
            return DivRem(left, right, out var _);
        }

        public static BigInteger512 operator *(in BigInteger256 left, in BigInteger256 right) {
            var ah = left.High;
            var al = left.Low;
            var bh = right.High;
            var bl = right.Low;

            var zero = new BigInteger256(0);
            var x0 = new BigInteger512(Mul128(al, bl), zero);
            var x1 = new BigInteger512(Mul128(al, bh), zero) + new BigInteger512(Mul128(ah, bl), zero);
            x1.AssignLeftShiftQuarter();
            var x2 = new BigInteger512(zero, Mul128(ah, bh));

            return x0 + x1 + x2;
        }

        private static BigInteger256 Mul128(UInt128 left, UInt128 right) {
            var ah = left >> 64;
            var al = (UInt128)(ulong)left;
            var bh = right >> 64;
            var bl = (UInt128)(ulong)right;

            var x0 = new BigInteger256(al * bl, 0);
            var x1 = new BigInteger256(al * bh, 0) + new BigInteger256(ah * bl, 0);
            x1.AssignLeftShiftQuarter();
            var x2 = new BigInteger256(0, ah * bh);

            return x0 + x1 + x2;
        }

        private static UInt128 Mul64(ulong left, ulong right) {
            var ah = left >> 32;
            var al = (ulong)(uint)left;
            var bh = right >> 32;
            var bl = (ulong)(uint)right;

            var x0 = new UInt128(0, al * bl);
            var x1 = (new UInt128(0, al * bh) + new UInt128(0, ah * bl)) << 32;
            var x2 = new UInt128(ah * bh, 0);

            return x0 + x1 + x2;
        }

        public static BigInteger256 operator %(in BigInteger256 left, in BigInteger256 right) {
            DivRem(left, right, out var reminder);
            return reminder;
        }

        public static BigInteger256 operator +(in BigInteger256 left, in BigInteger256 right) {
            var res = new BigInteger256(left);
            res.AssignAdd(right);
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

        public static BigInteger256 DivRem(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            return DivRemBits(dividend, divisor, out remainder);
            // var res = BigInteger.DivRem(dividend.ToNative(), divisor.ToNative(), out var rem);
            // remainder = new BigInteger256(rem);
            // return new BigInteger256(res);
        }

        public static BigInteger256 DivRemBits(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 remainder) {
            var result = new BigInteger256();
            var value = new BigInteger512(dividend);
            var bit = BITS_SIZE - 1;
            for (var i = 0; i < BITS_SIZE; i++) {
                value.AssignLeftShift();
                if (value.High >= divisor) {
                    value.High.AssignSub(divisor);
                    result.SetBit(bit);
                }
                bit--;
            }
            remainder = value.High;
            return result;
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

        public readonly string ToHexUnsigned(int length = 32) {
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

        public readonly void WriteBigEndian(Span<byte> buffer) {
            var ptr = 0;
            for (var i = ITEMS_SIZE - 1; i >= 0; i--) {
                var val = Data[i];
                buffer[ptr + 3] = (byte)(val & 0xff);
                val >>= 8;
                buffer[ptr + 2] = (byte)(val & 0xff);
                val >>= 8;
                buffer[ptr + 1] = (byte)(val & 0xff);
                val >>= 8;
                buffer[ptr] = (byte)val;
                ptr += 4;
            }
        }

        public override int GetHashCode() {
            uint res = 0;
            for (var i = 1; i < ITEMS_SIZE; i++) {
                res ^= Data[i];
            }
            return (int)res;
        }

        //todo: one
        //todo: zero
    }
}
