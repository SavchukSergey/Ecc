using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ecc {
    public readonly struct ECSignature {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ECSignature(BigInteger r, BigInteger s, ECCurve curve) {
            R = r;
            S = s;
            Curve = curve;
        }

        public readonly BigInteger R;

        public readonly BigInteger S;

        public readonly ECCurve Curve;

        public readonly byte[] ToByteArray() {
            var order8 = (Curve.OrderSize + 7) / 8;
            var r = R.ToBigEndianBytes();
            var s = S.ToBigEndianBytes();
            var rlen = r.Length;
            var rstart = rlen - order8;
            var slen = s.Length;
            var sstart = slen - order8;
            var res = new byte[order8 * 2];
            for (var i = 0; i < order8; i++) {
                res[i] = r[rstart + i];
                res[i + order8] = s[sstart + i];
            }
            return res;
        }

        public readonly string ToHexString() {
            var arr = ToByteArray();
            return arr.ToHexString();
        }

    }
}
