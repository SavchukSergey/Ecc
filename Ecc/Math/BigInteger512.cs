using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Ecc.Math {
    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public unsafe partial struct BigInteger512 {

        public const int BITS_SIZE = 512;
        public const int BYTES_SIZE = BITS_SIZE / 8;
        internal const int UINT32_SIZE = BITS_SIZE / 32;
        internal const int UINT64_SIZE = BITS_SIZE / 64;

        public const int HEX_SIZE = BYTES_SIZE * 2;

        [FieldOffset(0)]
        public BigInteger256 Low;

        [FieldOffset(32)]
        public BigInteger256 High;

        [FieldOffset(16)]
        public BigInteger256 Middle; // for 256.256 fixed point arithmetics

        [FieldOffset(0)]
        public BigInteger128 BiLow128;

        [FieldOffset(0)]
        public BigInteger192 BiLow192;

        [FieldOffset(0)]
        public BigInteger256 BiLow256;

        [FieldOffset(0)]
        public BigInteger384 BiLow384;

        [FieldOffset(24)]
        public BigInteger384 BiHigh384;

        [FieldOffset(0)]
        internal fixed byte Bytes[BYTES_SIZE];

        [FieldOffset(0)]
        internal fixed uint UInt32[UINT32_SIZE];

        [FieldOffset(0)]
        internal fixed ulong UInt64[UINT64_SIZE];

        [FieldOffset(56)]
        internal ulong HighUInt64;

        [FieldOffset(0)]
        public uint LowUInt32;

        [FieldOffset(0)]
        public ulong LowUInt64;

        [FieldOffset(0)]
        internal UInt128 UInt128_0;
        [FieldOffset(16)]
        internal UInt128 UInt128_1;
        [FieldOffset(32)]
        internal UInt128 UInt128_2;
        [FieldOffset(48)]
        internal UInt128 UInt128_3;

        [FieldOffset(0)]
        internal BigInteger128 Low128;

        [FieldOffset(48)]
        internal UInt128 HighUInt128;

        [FieldOffset(48)]
        internal BigInteger128 BiHigh128;

        [FieldOffset(32)]
        internal BigInteger256 BiHigh256;

        public readonly byte GetByte(int index) {
            return Bytes[index];
        }

        public readonly BigInteger ToNative() {
            Span<byte> array = stackalloc byte[BYTES_SIZE];
            for (var i = 0; i < BYTES_SIZE; i++) {
                var bt = Bytes[i];
                array[i] = bt;
            }
            return new BigInteger(array, isUnsigned: true, isBigEndian: false);
        }

        public readonly bool IsZero {
            get {
                return Low.IsZero && High.IsZero;
            }
        }

        public readonly bool IsOne {
            get {
                return Low.IsOne && High.IsZero;
            }
        }

        public readonly BigInteger512 Clone() {
            return new BigInteger512(Low, High);
        }

        public void Clear() {
            for (var i = 0; i < UINT32_SIZE; i++) {
                UInt32[i] = 0;
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

        public void AssignAddHigh(in BigInteger256 other) {
            High += other;
        }

        public bool AssignSub(in BigInteger512 other) {
            bool carry = false;
            for (var i = 0; i < UINT64_SIZE; i++) {
                UInt128 acc = UInt64[i];
                acc -= other.UInt64[i];
                acc -= carry ? 1u : 0u;
                UInt64[i] = (ulong)acc;
                carry = acc > ulong.MaxValue; //todo: use shift to avoid branching
            }
            return carry;
        }

        public void AssignSub(in BigInteger256 other) {
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

        public void AssignSub(in BigInteger192 other) {
            Int128 acc = 0;

            for (var i = 0; i < 3; i++) {
                acc += UInt64[i];
                acc -= other.UInt64[i];
                UInt64[i] = (ulong)acc;
                acc >>= 64;
            }

            for (var i = 3; i < 8; i++) {
                acc += UInt64[i];
                UInt64[i] = (ulong)acc;
                acc >>= 64;
            }
        }

        public void AssignSub(in BigInteger128 other) {
            //todo:
            AssignSub(new BigInteger256(other));
        }

        public readonly BigInteger512 Sub(in BigInteger512 other, out bool carry) {
            carry = false;
            var res = new BigInteger512(this);
            for (var i = 0; i < UINT64_SIZE; i++) {
                UInt128 acc = UInt64[i];
                acc -= other.UInt64[i];
                acc -= carry ? 1u : 0u;
                res.UInt64[i] = (ulong)acc;
                carry = acc > ulong.MaxValue; //todo: use shift to avoid branching
            }
            return res;
        }

        public void AssignDecrement() {
            if (Low.IsZero) {
                High.AssignDecrement();
            }
            Low.AssignDecrement();
        }

        public void AssignIncrement() {
            Low.AssignIncrement();
            if (Low.IsZero) {
                High.AssignIncrement();
            }
        }

        public void AssignNegate() {
            bool carry = false;
            ulong add = 1ul;
            for (var i = 0; i < UINT64_SIZE; i++) {
                UInt128 acc = 0xffffffff_fffffffful;
                acc -= UInt64[i];
                acc -= carry ? 1u : 0u;
                acc += add;
                add = 0;
                UInt64[i] = (ulong)acc;
                carry = acc > ulong.MaxValue; //todo: use shift to avoid branching
            }
        }

        public static BigInteger512 operator +(in BigInteger512 left, in BigInteger512 right) {
            var res = new BigInteger512(left);
            res.AssignAdd(right);
            return res;
        }

        public static BigInteger512 operator +(in BigInteger512 left, in BigInteger256 right) {
            var res = new BigInteger512(left);
            res.AssignAdd(right);
            return res;
        }

        public static BigInteger512 operator -(in BigInteger512 left, in BigInteger512 right) {
            var res = new BigInteger512(left);
            res.AssignSub(right);
            return res;
        }

        public static BigInteger512 operator -(in BigInteger512 left, in BigInteger256 right) {
            var res = new BigInteger512(left);
            res.AssignSub(right);
            return res;
        }

        public readonly int LeadingZeroCount() {
            var h = High.LeadingZeroCount();
            if (h != BigInteger256.BITS_SIZE) {
                return h;
            }
            var l = Low.LeadingZeroCount();
            return l + BigInteger256.BITS_SIZE;
        }

        public readonly bool TryWrite(Span<byte> buffer) {
            if (buffer.Length < BYTES_SIZE) {
                return false;
            }
            for (var i = 0; i < BYTES_SIZE; i++) {
                buffer[i] = Bytes[i];
            }
            return true;
        }

        public static BigInteger512 Gcd(in BigInteger512 a, in BigInteger256 b) {
            return new BigInteger512(BigInteger.GreatestCommonDivisor(a.ToNative(), b.ToNative()));
        }

        public override int GetHashCode() {
            uint res = 0;
            for (var i = 1; i < UINT32_SIZE; i++) {
                res ^= UInt32[i];
            }
            return (int)res;
        }

    }
}
