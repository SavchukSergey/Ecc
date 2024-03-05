using System;

namespace Ecc.Math {
    public unsafe ref struct BigInteger512 {

        public const int BITS_SIZE = 512;
        public const int BYTES_SIZE = BITS_SIZE / 8;
        private const int ITEM_BITS_SIZE = 32;
        internal const int ITEMS_SIZE = BITS_SIZE / ITEM_BITS_SIZE;

        internal fixed uint Data[ITEMS_SIZE];

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
            ZeroExtendFrom(value);
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

        public void Clear() {
            for (var i = 0; i < ITEMS_SIZE; i++) {
                Data[i] = 0;
            }
        }

        public bool Add(in BigInteger512 other) {
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
