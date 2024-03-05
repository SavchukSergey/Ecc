using System.Runtime.CompilerServices;
using Ecc.Math;

namespace Ecc {
    public readonly struct ECSignature256 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ECSignature256(BigInteger256 r, BigInteger256 s, ECCurve256 curve) {
            R = r;
            S = s;
            Curve = curve;
        }

        public readonly BigInteger256 R;

        public readonly BigInteger256 S;

        public readonly ECCurve256 Curve;

        public readonly byte[] ToByteArray() {
            var order8 = (Curve.OrderSize + 7) / 8;
            var r = R.ToBigEndianBytes(); //todo: provide curve order as array length
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
