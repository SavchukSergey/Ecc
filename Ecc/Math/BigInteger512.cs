using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Ecc.Math {
    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public unsafe partial struct BigInteger512 {

        public const int BITS_SIZE = 512;
        public const int BYTES_SIZE = BITS_SIZE / 8;
        private const int ITEM_BITS_SIZE = 32;
        [Obsolete]
        internal const int ITEMS_SIZE = BITS_SIZE / ITEM_BITS_SIZE;
        internal const int UINT32_SIZE = BITS_SIZE / 32;
        internal const int UINT64_SIZE = BITS_SIZE / 64;

        [Obsolete]
        [FieldOffset(0)]
        internal fixed uint Data[ITEMS_SIZE]; //todo: review usages

        [FieldOffset(0)]
        public BigInteger256 Low;

        [FieldOffset(32)]
        public BigInteger256 High;

        [FieldOffset(16)]
        public BigInteger256 Middle; // for 256.256 fixed point arithmetics

        [FieldOffset(0)]
        internal fixed byte Bytes[BYTES_SIZE];

        [FieldOffset(0)]
        internal fixed uint UInt32[UINT32_SIZE];

        [FieldOffset(0)]
        internal fixed ulong UInt64[UINT64_SIZE];

        public readonly byte GetByte(int index) {
            return Bytes[index];
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

        public readonly bool IsOne {
            get {
                return Low.IsOne && High.IsZero;
            }
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

        public readonly BigInteger512 ModInverse(in BigInteger512 modulus) {
            return new BigInteger512(ToNative().ModInverse(modulus.ToNative()));
        }

        public static BigInteger256 operator %(in BigInteger512 left, in BigInteger256 right) {
            return new BigInteger256(left.ToNative() % right.ToNative());
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
