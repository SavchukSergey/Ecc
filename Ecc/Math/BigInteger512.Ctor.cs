using System;
using System.Numerics;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public BigInteger512() {
            for (var i = 0; i < UINT64_SIZE; i++) {
                UInt64[i] = 0;
            }
        }

        public BigInteger512(uint value) {
            UInt32[0] = value;
            for (var i = 1; i < UINT32_SIZE; i++) {
                UInt32[i] = 0;
            }
        }

        public BigInteger512(ulong value) {
            UInt64[0] = value;
            for (var i = 1; i < UINT64_SIZE; i++) {
                UInt64[i] = 0;
            }
        }

        public BigInteger512(BigInteger128 value) {
            Low.BiLow = value;
            Low.BiHigh = new BigInteger128();
            High = new BigInteger256();
        }

        public BigInteger512(in BigInteger256 value) {
            Low = value;
            High = new BigInteger256(0);
        }

        public BigInteger512(in BigInteger256 low, in BigInteger256 high) {
            Low = low;
            High = high;
        }

        public BigInteger512(in BigInteger512 other) {
            Low = other.Low;
            High = other.High;
        }

        public BigInteger512(UInt128 l0) {
            Low.Low = l0;
            Low.High = 0;
            High = new BigInteger256(0);
        }

        [Obsolete]
        public BigInteger512(in BigInteger value) {
            var data = value.ToByteArray(isBigEndian: false);
            for (var i = 0; i < BYTES_SIZE; i++) {
                Bytes[i] = i < data.Length ? data[i] : (byte)0;
            }
        }

    }
}