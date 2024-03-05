using System;
using System.Numerics;

namespace Ecc {
    public readonly ref struct BigRefInteger {

        public required readonly Span<byte> Data { get; init; }

        public readonly int Sign {
            get {
                var len = Data.Length;
                if (len == 0) {
                    return 0;
                }
                var last = Data[len - 1];
                if ((last & 0x80) != 0) {
                    return -1;
                }
                return IsZero ? 0 : 1;
            }
        }

        public readonly bool IsZero {
            get {
                var len = Data.Length;
                for (var i = 0; i < len; i++) {
                    if (Data[i] != 0) {
                        return false;
                    }
                }
                return true;
            }
        }

        public static int Compare(in BigRefInteger left, in BigRefInteger right) {
            var leftLen = left.Data.Length;
            var rightLen = right.Data.Length;
            var len = System.Math.Max(leftLen, rightLen);
            for (var i = len - 1; i >= 0; i--) {
                var leftBt = i < leftLen ? left.Data[i] : 0;
                var rightBt = i < rightLen ? right.Data[i] : 0;
                if (leftBt > rightBt) {
                    return 1;
                }
                if (leftBt < rightBt) {
                    return -1;
                }
            }
            return 0;
        }

        public readonly BigInteger ToBigInteger() {
            return new BigInteger(Data, isUnsigned: true, isBigEndian: false);
        }

        public readonly void ToBigEndianBytes(Span<byte> target) {
            if (target.Length != Data.Length) {
                throw new Exception();
            }
            var ti = target.Length - 1;
            for (var i = 0; i < Data.Length; i++) {
                target[ti--] = Data[i];
            }
        }

        public static void ParseHexUnsigned(string src, ref BigRefInteger val) {
            if ((src.Length + 1) / 2 > val.Data.Length) {
                throw new Exception();
            }
            var di = 0;
            var si = src.Length - 1;
            while (si >= 1) {
                var l = StringUtils.GetHexDigit(src[si--]);
                var h = StringUtils.GetHexDigit(src[si--]);
                var bt = (h << 4) + l;
                val.Data[di] = (byte)bt;
                di++;
            }
            if (si >= 0) {
                var l = StringUtils.GetHexDigit(src[si]);
                val.Data[di] = l;
            }
        }

    }

}