using System;
using System.Buffers;
using System.Buffers.Text;
using System.Text;

namespace Ecc {
    public static class Base64Url {

        public static int GetCharCount(ReadOnlySpan<byte> src) {
            var full = src.Length / 3;
            var rest = src.Length % 3;
            return full * 4 + rest switch {
                0 => 0,
                1 => 2,
                2 => 3,
                _ => 0
            };
        }

        public static int GetCharCount(int length) {
            var full = length / 3;
            var rest = length % 3;
            return full * 4 + rest switch {
                0 => 0,
                1 => 2,
                2 => 3,
                _ => 0
            };
        }

        public static int GetByteCount(ReadOnlySpan<char> src) {
            var len = src.Length;
            var padding = 0;
            if (len > 0 && src[^1] == '=') {
                padding++;
            }
            if (len > 1 && src[^2] == '=') {
                padding++;
            }
            len -= padding;

            var full = len / 4;
            var rest = len % 4;

            return full * 3 + rest switch {
                0 => 0,
                2 => 1,
                3 => 2,
                _ => throw new FormatException("Unable to parse base64url string")
            };
        }

        public static int Decode(ReadOnlySpan<char> src, Span<byte> output) {
            var res = TryDecode(src, output, out var bytesWritten);
            if (res == System.Buffers.OperationStatus.InvalidData) {
                throw new FormatException("Unable to parse base64url string");
            }
            return bytesWritten;
        }

        public static OperationStatus TryDecode(ReadOnlySpan<char> src, Span<byte> output, out int bytesWritten) {
            var alignment = (src.Length % 4) switch {
                0 => 0, // No pad chars in this case
                2 => 2, // Two pad chars
                3 => 1, // One pad char
                _ => throw new FormatException("Unable to parse base64url string")
            };
            Span<byte> temp = stackalloc byte[src.Length + alignment];
            Encoding.ASCII.GetBytes(src, temp);
            temp.Replace((byte)'-', (byte)'+');
            temp.Replace((byte)'_', (byte)'/');
            while (alignment > 0) {
                temp[^alignment] = (byte)'=';
                alignment--;
            }
            return Base64.DecodeFromUtf8(temp, output, out var _, out bytesWritten);
        }

        public static string Encode(ReadOnlySpan<byte> data) {
            var charsCount = GetCharCount(data);
            Span<char> chars = stackalloc char[charsCount];
            var actualChars = Encode(data, chars);
            return new string(chars[..actualChars]);
        }

        public static int Encode(ReadOnlySpan<byte> data, Span<char> output) {
            var resSize = Base64.GetMaxEncodedToUtf8Length(data.Length);
            Span<byte> utf8 = stackalloc byte[resSize];
            Base64.EncodeToUtf8(data, utf8, out var _, out var byteWritten);
            for (var i = 0; i < byteWritten; i++) {
                var bt = utf8[i];
                if (bt == '=') {
                    return i;// Remove any trailing '='s
                } else if (bt == '+') {
                    output[i] = '-'; // 62nd char of encoding
                } else if (bt == '/') {
                    output[i] = '_'; // 63rd char of encoding
                } else {
                    output[i] = (char)bt;
                }
            }
            return byteWritten;
        }

    }
}
