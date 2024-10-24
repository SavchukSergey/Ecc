using System.Text;
using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public static BigInteger512 ParseHexUnsigned(ReadOnlySpan<char> str) {
            var res = new BigInteger512();
            if (str.Length >= 2) {
                if (str[0] == '0' && str[1] == 'x') {
                    str = str[2..];
                }
            }
            if (str.Length > HEX_SIZE) {
                throw new ArgumentException($"Expected hex string with {HEX_SIZE} characters, was {str.Length}");
            }
            var ptr = 0;
            var charPtr = str.Length - 1;
            for (var i = 0; i < UINT32_SIZE; i++) {
                uint part = 0;
                for (var j = 0; j < 32 && charPtr >= 0; j += 4) {
                    var hd = StringUtils.GetHexDigit(str[charPtr]);
                    part |= ((uint)hd << j);
                    charPtr--;
                }
                res.UInt32[ptr++] = part;
            }
            return res;
        }

        public readonly string ToHexUnsigned() {
            Span<char> chars = stackalloc char[HEX_SIZE];
            TryWriteHex(chars);
            return new string(chars);
        }

        public readonly bool TryWriteHex(Span<char> output) {
            if (output.Length < HEX_SIZE) {
                return false;
            }
            const string hex = "0123456789abcdef";
            var ptr = 0;
            for (var i = BYTES_SIZE - 1; i >= 0; i--) {
                var ch = GetByte(i);
                output[ptr++] = hex[ch >> 4];
                output[ptr++] = hex[ch & 0x0f];
            }
            return true;
        }

        public readonly string ToHexFixedPoint() {
            var sbLength = HEX_SIZE + 1;
            var sb = new StringBuilder(sbLength, sbLength);
            var dataLength = BYTES_SIZE;
            const string hex = "0123456789abcdef";
            for (var i = 63; i >= 0; i--) {
                if (i == 31) {
                    sb.Append('.');
                }
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