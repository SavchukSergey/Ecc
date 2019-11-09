using System;

namespace Ecc {
    public static class Base64Url {

        public static byte[] Decode(string src) {
            var s = src;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) {// Pad with trailing '='s
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default:
                    throw new Exception("Illegal base64url string!");
            }
            return Convert.FromBase64String(s); // Standard base64 decoder
        }

        public static string Encode(byte[] data) {
            return Encode(data, 0, data.Length);
        }

        public static string Encode(byte[] data, long offset, long length) {
            var s = Convert.ToBase64String(data, (int)offset, (int)length); // Regular base64 encoder
            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }
    }
}
