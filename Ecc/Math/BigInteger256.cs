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
        public const int UINT64_SIZE = BITS_SIZE / 64;
        public const int UINT32_SIZE = BITS_SIZE / 32;
        public const int UINT16_SIZE = BITS_SIZE / 16;

        public const int HEX_SIZE = BYTES_SIZE * 2;

        [FieldOffset(0)]
        public fixed byte Bytes[BYTES_SIZE];

        [FieldOffset(0)]
        public fixed ulong UInt64[UINT64_SIZE];

        [FieldOffset(0)]
        public fixed uint UInt32[UINT32_SIZE];

        [FieldOffset(0)]
        public fixed ulong UInt16[UINT16_SIZE];

        [FieldOffset(0)]
        public uint LowUInt32;
        [FieldOffset(0)]
        public ulong LowUInt64;
        [FieldOffset(0)]
        public UInt128 LowUInt128;

        [FieldOffset(28)]
        public uint HighUInt32;
        [FieldOffset(24)]
        public ulong HighUInt64;
        [FieldOffset(16)]
        public UInt128 HighUInt128;

        [FieldOffset(0)]
        public BigInteger128 BiLow128;
        [FieldOffset(0)]
        public BigInteger192 BiLow192;

        [FieldOffset(16)]
        public BigInteger128 BiHigh128;

        [FieldOffset(8)]
        public BigInteger192 BiHigh192;

        [FieldOffset(8)]
        public BigInteger128 BiMiddle128;

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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return LowUInt128 == 0 && HighUInt128 == 0;
            }
        }

        public readonly bool IsOne {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return LowUInt128 == 1 && HighUInt128 == 0;
            }
        }

        public readonly bool IsEven {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return (Bytes[0] & 0x1) == 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly BigInteger256 Clone() {
            return new BigInteger256(LowUInt128, HighUInt128);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() {
            LowUInt128 = 0;
            HighUInt128 = 0;
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

        public readonly BigInteger256 Double() {
            var res = Clone();
            res.AssignDouble();
            return res;
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

        public void AssignNegate() {
            var borrow = LowUInt128 != 0;
            LowUInt128 = -LowUInt128;
            HighUInt128 = -HighUInt128;
            if (borrow) {
                HighUInt128--;
            }
        }

        public void AssignDecrement() {
            if (LowUInt128 == 0) {
                HighUInt128--;
            }
            LowUInt128--;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AssignIncrement() {
            LowUInt128++;
            if (LowUInt128 == 0) {
                HighUInt128++;
                if (HighUInt128 == 0) {
                    return true;
                }
            }
            return false;
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
            var borrow = res.LowUInt128 < other.LowUInt128;
            res.LowUInt128 -= other.LowUInt128;
            if (borrow) {
                borrow = res.HighUInt128 == 0;
                res.HighUInt128--;
            }
            borrow |= res.HighUInt128 < other.HighUInt128;
            res.HighUInt128 -= other.HighUInt128;
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
            Mul(in this, in other, out var temp);
            return temp % modulus;
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
            var mont = ctx.ToMontgomery(this);
            ctx.ModPow(in mont, in exp, out var newMont);
            return ctx.Reduce(in newMont);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly BigInteger256 ModSquare(in BigInteger256 modulus) {
            ModSquare(in modulus, out var result);
            return result;
        }

        public readonly void ModSquare(in BigInteger256 modulus, out BigInteger256 result) {
            Square(out var square);
            BigInteger512.DivRem(square, modulus, out var _, out result);
        }

        public readonly BigInteger256 ModCube(in BigInteger256 modulus) {
            return ModMul(ModSquare(modulus), modulus);
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

        public static BigInteger256 Gcd(in BigInteger256 a, in BigInteger256 b) {
            return EuclidExtended(a, b).Gcd;
        }

        //todo: one
        //todo: zero
    }
}
