using System.Text;
using System;

namespace Ecc.Math {
    public unsafe partial struct BigInteger128 {

        public static BigInteger128 ParseHexUnsigned(ReadOnlySpan<char> val) {
            var res = new BigInteger128();
            res.ReadFromHex(val);
            return res;
        }

        public void ReadFromHex(ReadOnlySpan<char> str) {
            if (str.StartsWith("0x")) {
                str = str[2..];
            }
            if (str.Length > BYTES_SIZE * 2) {
                throw new ArgumentException($"Expected hex string with {BYTES_SIZE * 2} characters");
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
                UInt32[ptr++] = part;
            }
        }

        public readonly string ToHexUnsigned(int length = 16) {
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

        public readonly string ToHexFixedPoint() {
            var sbLength = 65;
            var sb = new StringBuilder(sbLength, sbLength);
            var dataLength = BYTES_SIZE;
            const string hex = "0123456789abcdef";
            for (var i = 31; i >= 0; i--) {
                if (i == 15) {
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