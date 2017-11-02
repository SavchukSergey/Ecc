using System.Text;

namespace Ecc {
    public static class ECPointExt {

        public static string GetCompressedHex(this ECPoint point) {
            if (point == null) return "00";
            var sb = new StringBuilder();
            if (point.Y.IsEven) sb.Append("02");
            else sb.Append("03");
            sb.Append(point.X.ToString("x"));
            return sb.ToString();
        }

    }
}