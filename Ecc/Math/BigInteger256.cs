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
        internal const int UINT64_SIZE = BITS_SIZE / 64;
        internal const int UINT32_SIZE = BITS_SIZE / 32;
        internal const int UINT16_SIZE = BITS_SIZE / 16;

        public const int HEX_SIZE = BYTES_SIZE * 2;

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


        [FieldOffset(24)]
        internal ulong HighUInt64;

        [FieldOffset(16)]
        internal UInt128 HighUInt128;

        [FieldOffset(0)]
        public BigInteger128 BiLow;

        [FieldOffset(16)]
        public BigInteger128 BiHigh;

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
            return (UInt32[btIndex] & (1 << (index & 0x1f))) != 0;
        }

        public void SetBit(int index) {
            var btIndex = index >> 5;
            UInt32[btIndex] |= (uint)(1 << (index & 0x1f));
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
                return (Bytes[0] & 0x1) == 0;
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

        public bool AssignDouble() {
            ulong carry = 0;
            for (var i = 0; i < UINT64_SIZE; i++) {
                var acc = UInt64[i];
                UInt64[i] = (acc << 1) + carry;
                carry = acc >> 63;
            }
            return carry > 0;
        }

        public void AssignAddHigh(BigInteger128 other) {
            High += other.UInt128;
        }

        public void AssignNegate() {
            var borrow = Low != 0;
            Low = -Low;
            High = -High;
            if (borrow) {
                BiHigh.AssignDecrement();
            }
        }

        public void AssignDecrement() {
            if (BiLow.IsZero) {
                BiHigh.AssignDecrement();
            }
            BiLow.AssignDecrement();
        }

        public void AssignIncrement() {
            BiLow.AssignIncrement();
            if (BiLow.IsZero) {
                BiHigh.AssignIncrement();
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
                if (exp.GetBit(bit)) {
                    acc = acc.ModMul(walker, modulus);
                }
                walker = walker.ModSquare(modulus);
            }
            return acc;
        }

        public readonly BigInteger256 ModPow(in BigInteger256 exp, in MontgomeryContext256 ctx) {
            var acc = ctx.One;
            var walker = ctx.ToMontgomery(this);
            for (var bit = 0; bit < BITS_SIZE; bit++) {
                if (exp.GetBit(bit)) {
                    acc = ctx.ModMul(acc, walker);
                }
                walker = ctx.ModSquare(walker);
            }
            return ctx.Reduce(acc);
        }

        public readonly BigInteger256 ModSquare(in BigInteger256 modulus) {
            //todo: square can use just 3 multiplications instead of regular four
            return ModMul(this, modulus);
        }

        public readonly BigInteger256 ModCube(in BigInteger256 modulus) {
            return ModMul(ModMul(this, modulus), modulus);
        }

        public readonly BigInteger256 ModDouble(in BigInteger256 modulus) {
            return ModAdd(this, modulus);
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
            return BITS_SIZE - LeadingZeroCount();
        }

        public readonly bool TryWrite(Span<byte> buffer) {
            if (buffer.Length < BYTES_SIZE) {
                return false;
            }
            for (var i = 0; i < BYTES_SIZE; i++) {
                var val = Bytes[i];
                buffer[i] = val;
            }
            return true;
        }

        public readonly void WriteBigEndian(Span<byte> buffer) {
            var ptr = 0;
            for (var i = BYTES_SIZE - 1; i >= 0; i--) {
                var val = Bytes[i];
                buffer[ptr++] = val;
            }
        }

        public override int GetHashCode() {
            uint res = 0;
            for (var i = 1; i < UINT32_SIZE; i++) {
                res ^= UInt32[i];
            }
            return (int)res;
        }

        public static BigInteger256 Gcd(in BigInteger256 a, in BigInteger256 b) {
            return EuclidExtended(a, b).Gcd;
        }

        //todo: one
        //todo: zero
    }
}
