using System;
using System.Numerics;

namespace Ecc {
    public readonly ref struct BigRefInteger {

        public readonly ReadOnlySpan<byte> Data { get; init; }

        public static int Compare(in BigRefInteger left, in BigRefInteger right) {
            var leftLen = left.Data.Length;
            var rightLen = right.Data.Length;
            var len = Math.Max(leftLen, rightLen);
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
            return new BigInteger(Data, isUnsigned: true);
        }

    }

}