using System.Numerics;

namespace Ecc {
    public struct BigUnsignedInteger {

        public readonly uint[] Data;

        public BigUnsignedInteger(uint value, int capacity) {
            Data = new uint[(capacity + 31) / 32];
            if (Data.Length > 0) {
                Data[0] = value;
            }
        }

        public BigUnsignedInteger(uint[] data) {
            Data = data;
        }

        public BigUnsignedInteger(byte[] data) {
            Data = new uint[(data.Length + 3) / 4];
            var full = data.Length & 0x03;
            for (var i = 0; i < full; i += 4) {
                var val = 0u;
                val += data[i + 3];
                val <<= 8;
                val += data[i + 2];
                val <<= 8;
                val += data[i + 1];
                val <<= 8;
                val += data[i];
                Data[i >> 2] = val;
            }
            var left = data.Length - full;
            if (left > 0) {
                ref uint last = ref Data[Data.Length - 1];
                if (left == 1) {
                    last = data[full];
                } else if (left == 2) {
                    last = (uint)(data[full] + (data[full + 1] << 8));
                } else if (left == 3) {
                    last = (uint)(data[full] + (data[full + 1] << 8) + (data[full + 2] << 16));
                }
            }
        }

        public uint Add(in BigUnsignedInteger other) {
            var acc = 0ul;
            var left = Data;
            var right = other.Data;
            for (var i = 0; i < left.Length; i++) {
                acc += left[i];
                acc += right[i];
                left[i] = (uint)acc;
                acc >>= 32;
            }
            return (uint)acc;
        }

        public BigInteger ToNative() {
            return new BigInteger(ToUint8());
        }

        public byte[] ToUint8() {
            var source = Data;
            var target = new byte[source.Length * 4];
            var offset = 0;
            for (var i = 0; i < source.Length; i++) {
                var val = source[i];
                target[offset++] = (byte)(val & 0xff);
                target[offset++] = (byte)((val >> 8) & 0xff);
                target[offset++] = (byte)((val >> 16) & 0xff);
                target[offset++] = (byte)((val >> 24) & 0xff);
            }
            return target;
        }

    }
}