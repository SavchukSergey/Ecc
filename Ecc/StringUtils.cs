using System;
using System.Text;

namespace Ecc {
    public static class StringUtils {

        private const string HEX = "0123456789abcdef";

        public static string ToHexString(this byte[] data) {
            var sb = new StringBuilder(data.Length * 2);
            return data.ToHexString(sb).ToString();
        }

        public static StringBuilder ToHexString(this byte[] data, StringBuilder sb) {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            foreach (var bt in data) {
                sb.Append(HEX[bt >> 4]);
                sb.Append(HEX[bt & 0x0f]);
            }
            return sb;
        }

        public static byte[] ToBytesFromHex(this string hex) {
            var res = new byte[hex.Length / 2];

            for (var i = 0; i < hex.Length; i++) {
                var ch = hex[i];
                res[i >> 1] <<= 4;
                res[i >> 1] |= GetHexDigit(ch);
            }

            return res;
        }

        private static byte GetHexDigit(char ch) {
            if (ch >= '0' && ch <= '9') return (byte)(ch - '0');
            if (ch >= 'A' && ch <= 'F') return (byte)(ch - 'A' + 10);
            if (ch >= 'a' && ch <= 'f') return (byte)(ch - 'a' + 10);
            throw new ArgumentOutOfRangeException(nameof(ch));
        }

    }
}
