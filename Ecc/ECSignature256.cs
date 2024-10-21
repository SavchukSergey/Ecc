using System;
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

        public readonly int GetSignatureByteSize() {
            var order8 = (Curve.OrderSize + 7) / 8;
            return (int)(order8 * 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetSignatureHexSize() {
            return GetSignatureByteSize() * 2;
        }

        public readonly bool TryWriteBytes(Span<byte> bytes) {
            var size = GetSignatureByteSize();
            if (bytes.Length < size) {
                return false;
            }
            bytes.Clear();
            R.ToBigEndianBytes(bytes[..(size / 2)]);
            S.ToBigEndianBytes(bytes[(size / 2)..]);
            return true;
        }

        public readonly string ToHexString() {
            Span<char> signatureChars = stackalloc char[GetSignatureHexSize()];
            ToHexString(signatureChars);
            return new string(signatureChars);
        }

        public readonly void ToHexString(Span<char> output) {
            Span<byte> signature = stackalloc byte[GetSignatureByteSize()];
            TryWriteBytes(signature);
            ((ReadOnlySpan<byte>)signature).ToHexString(output);
        }
    }
}
