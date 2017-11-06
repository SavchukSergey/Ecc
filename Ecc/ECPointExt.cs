using System.Text;

namespace Ecc {
    public static class ECPointExt {

        public static string GetCompressedHex(this ECPoint point) {
            if (point == null) return "00";
            var keySize = point.Curve.KeySize8;
            var sb = new StringBuilder();
            if (point.Y.IsEven) sb.Append("02");
            else sb.Append("03");
            sb.Append(point.X.ToHexUnsigned(keySize));
            return sb.ToString();
        }

        public static string GetHex(this ECPoint point, bool compress = true) {
            if (compress) return GetCompressedHex(point);
            var keySize = point.Curve.KeySize8;
            return $"04{point.X.ToHexUnsigned(keySize)}{point.Y.ToHexUnsigned(keySize)}";
        }

    }
}