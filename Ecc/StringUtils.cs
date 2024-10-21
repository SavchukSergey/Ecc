using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ecc {
    public static class StringUtils {

        private const string HEX = "0123456789abcdef";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHexCharsCount(this ReadOnlySpan<byte> bytes) {
            return bytes.Length * 2;
        }
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHexBytesCount(this ReadOnlySpan<char> bytes) {
            return bytes.Length / 2;
        }

        public static void ToHexString(this ReadOnlySpan<byte> data, Span<char> output) {
            for (var i = 0; i < data.Length; i++) {
                var bt = data[i];
                output[i * 2] = HEX[bt >> 4];
                output[i * 2 + 1] = HEX[bt & 0x0f];
            }
        }

        public static void ToBytesFromHex(this ReadOnlySpan<char> hex, Span<byte> output) {
            for (var i = 0; i < hex.Length; i++) {
                var ch = hex[i];
                output[i >> 1] <<= 4;
                output[i >> 1] |= GetHexDigit(ch);
            }
        }

        public static byte GetHexDigit(char ch) {
            if (ch >= '0' && ch <= '9') return (byte)(ch - '0');
            if (ch >= 'A' && ch <= 'F') return (byte)(ch - 'A' + 10);
            if (ch >= 'a' && ch <= 'f') return (byte)(ch - 'a' + 10);
            throw new ArgumentOutOfRangeException(nameof(ch));
        }

    }
}
