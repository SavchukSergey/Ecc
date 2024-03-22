using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ecc.Math {
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    [SkipLocalsInit]
    public unsafe partial struct BigInteger256 {

        public const int BITS_SIZE = 256;
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
        internal fixed ulong UInt64[UINT64_SIZE];

        [FieldOffset(0)]
        internal fixed uint UInt32[UINT32_SIZE];

        [FieldOffset(0)]
        internal fixed ulong UInt16[UINT16_SIZE];

        [FieldOffset(0)]
        public UInt128 Low;

        [FieldOffset(16)]
        public UInt128 High;

        public BigInteger256() {
            Low = 0;
            High = 0;
        }

        public BigInteger256(uint value) {
            Low = value;
            High = 0;
        }

        public BigInteger256(uint b0, uint b1) {
            UInt32[0] = b0;
            UInt32[1] = b1;
            UInt64[1] = 0;
            High = 0;
        }

        public BigInteger256(uint b0, uint b1, uint b2, uint b3) {
            UInt32[0] = b0;
            UInt32[1] = b1;
            UInt32[2] = b2;
            UInt32[3] = b3;
            High = 0;
        }

        public BigInteger256(ulong b0, ulong b1, ulong b2, ulong b3) {
            UInt64[0] = b0;
            UInt64[1] = b1;
            UInt64[2] = b2;
            UInt64[3] = b3;
        }

        public BigInteger256(uint b0, uint b1, uint b2, uint b3, uint b4, uint b5, uint b6, uint b7) {
            UInt32[0] = b0;
            UInt32[1] = b1;
            UInt32[2] = b2;
            UInt32[3] = b3;
            UInt32[4] = b4;
            UInt32[5] = b5;
            UInt32[6] = b6;
            UInt32[7] = b7;
        }

        public BigInteger256(ulong value) {
            UInt64[0] = value;
            UInt64[1] = 0;
            UInt64[2] = 0;
            UInt64[3] = 0;
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

        public BigInteger256(in BigInteger value) {
            var data = value.ToByteArray(isBigEndian: false);
            for (var i = 0; i < BYTES_SIZE; i++) {
                Bytes[i] = i < data.Length ? data[i] : (byte)0;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly byte GetByte(int index) {
            return Bytes[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetByte(int index, byte value) {
            Bytes[index] = value;
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

        public readonly bool IsOne {
            get {
                return Low == 1 && High == 0;
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
            UInt128 carry = 0;
            for (var i = 0; i < UINT64_SIZE; i++) {
                UInt128 acc = UInt64[i];
                acc += other.UInt64[i];
                acc += carry;
                UInt64[i] = (ulong)acc;
                carry = acc >> 64;
            }
            if (carry > 0 || this >= modulus) {
                AssignSub(modulus);
            }
        }

        public void AssignModDouble(in BigInteger256 modulus) {
            ulong carry = 0;
            for (var i = 0; i < UINT64_SIZE; i++) {
                var acc = UInt64[i];
                UInt64[i] = (acc << 1) + carry;
                carry = acc >> 63;
            }

            if (carry > 0 || this >= modulus) {
                AssignSub(modulus);
            }
        }

        public bool AssignAdd(in BigInteger256 other) {
            UInt128 carry = 0;
            for (var i = 0; i < UINT64_SIZE; i++) {
                UInt128 acc = UInt64[i];
                acc += other.UInt64[i];
                acc += carry;
                UInt64[i] = (ulong)acc;
                carry = acc >> 64;
            }
            return carry > 0;
        }

        public bool AssignDouble() {
            ulong carry = 0;
            for (var i = 0; i < UINT64_SIZE; i++) {
                var acc = UInt64[i];
                UInt64[i] = (acc << 1) + carry;
                carry = acc >> 63;
            }
            return carry > 0;
        }

        public void AssignAddHigh(UInt128 other) {
            High += other;
        }

        public void AssignNegate() {
            var borrow = Low != 0;
            Low = -Low;
            High = -High;
            if (borrow) {
                High--;
            }
        }

        public void AssignDecrement() {
            if (Low == 0) {
                High--;
            }
            Low--;
        }
        public void AssignIncrement() {
            Low++;
            if (Low == 0) {
                High++;
            }
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
            var res = this;
            var borrow = res.Low < other.Low;
            res.Low -= other.Low;
            if (borrow) {
                borrow = res.High == 0;
                res.High--;
            }
            borrow |= res.High < other.High;
            res.High -= other.High;
            if (borrow) {
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

        public static BigInteger256 MulModMontgomery(in BigInteger256 left, in BigInteger256 right, in BigInteger256 modulus, in BigInteger256 reciprocalModulus, int shift) {
            var mul512 = (left * right);
            var q1024 = mul512 * reciprocalModulus;
            q1024.AssignRightShift(shift);

            var q256 = q1024.Low.Low;
            var remainder512 = (mul512 - q256 * modulus);

            if (remainder512 > modulus) {
                remainder512 -= modulus;
                q256.AssignIncrement();
            }

            if (remainder512 > modulus) {
                remainder512 -= modulus;
                q256.AssignIncrement();
            }

            return remainder512.Low;
        }


        public readonly BigInteger256 ModMul(in BigInteger256 other, in BigInteger256 modulus) {
            //todo: optimize
            return ModMulBit(other, modulus);
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
            return BigInteger256Ext.EuclidExtended(this, modulus).X;
        }

        public readonly BigInteger ToNative() {
            Span<byte> array = stackalloc byte[BYTES_SIZE];
            for (var i = 0; i < BYTES_SIZE; i++) {
                var bt = Bytes[i];
                array[i] = bt;
            }
            return new BigInteger(array, isUnsigned: true, isBigEndian: false);
        }

        public static BigInteger256 operator /(in BigInteger256 left, in BigInteger256 right) {
            return DivRem(left, right, out var _);
        }

        private static BigInteger256 Mul128(UInt128 left, UInt128 right) {
            if (left == 0 || right == 0) {
                return new BigInteger256(0);
            }
            if (left == 1) {
                return new BigInteger256(right);
            }
            if (right == 1) {
                return new BigInteger256(left);
            }
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

        private static BigInteger256 Mul128(UInt128 left, ulong right) {
            var ah = left >> 64;
            var al = (UInt128)(ulong)left;
            var bl = (UInt128)right;

            var x0 = new BigInteger256(al * bl, 0);
            var x1 = new BigInteger256(ah * bl, 0);
            x1.AssignLeftShiftQuarter();

            return x0 + x1;
        }

        private static UInt128 Mul128Low(UInt128 left, UInt128 right) {
            var ah = left >> 64;
            var al = (UInt128)(ulong)left;
            var bh = right >> 64;
            var bl = (UInt128)(ulong)right;

            var x0 = al * bl;
            var x1 = (al * bh + ah * bl) << 64; //todo: we can use Mul64Low here check if it is faster

            return x0 + x1;
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
            DivRem(left, right, out var remainder);
            return remainder;
        }

        public static BigInteger256 operator +(in BigInteger256 left, in BigInteger256 right) {
            var res = new BigInteger256(left);
            res.AssignAdd(right);
            return res;
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

        public readonly int PopCount() {
            var val = 0;
            for (var i = UINT64_SIZE - 1; i >= 0; i--) {
                val += BitOperations.PopCount(UInt64[i]);
            }
            return val;
        }

        public readonly int Log2() {
            //BitOperations.LeadingZeroCount(UInt64[3]); //todo
            int res = BITS_SIZE;
            for (var i = ITEMS_SIZE - 1; i >= 0; i--) {
                var item = GetItem(i); //todo: use UInt128 items
                var mask = 0x8000_0000;
                while (mask != 0) {
                    if ((item & mask) != 0) {
                        return res;
                    }
                    mask >>= 1;
                    res--;
                }
            }
            return res;
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
