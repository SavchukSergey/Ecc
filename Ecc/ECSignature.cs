using System;
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
