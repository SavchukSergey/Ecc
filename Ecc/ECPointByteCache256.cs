using System;
using System.IO;
using Ecc.Math;

namespace Ecc {
    public class ECPointByteCache256 {

        private readonly ECPoint256[] _points;

        private readonly int _keyBytesCount;

        private readonly ECCurve256 _curve;

        public const int TOTAL_POINTS_COUNT = 256 * 256 / 8; // 8192 points
        public const int POINT_DATA_SIZE = 2 * 256 / 8;      // 64 bytes: two coords (32 bytes each) for each point
        public const int TOTAL_POINTS_BYTES = TOTAL_POINTS_COUNT * POINT_DATA_SIZE; //0.5Mb

        public ECPointByteCache256(in ECPoint256 generator, int keySize) {
            _curve = generator.Curve;
            var keyBytesCount = (keySize + 7) / 8;
            _keyBytesCount = keyBytesCount;
            _points = new ECPoint256[keyBytesCount * 256];
            var shiftedGenerator = generator;

            for (var i = 0; i < keyBytesCount; i++) {
                var point = generator.Curve.Infinity;
                for (var j = 0; j < 256; j++) {
                    _points[i * 256 + j] = point;
                    point = point + shiftedGenerator;
                }
                shiftedGenerator = point;
            }
        }

        private ECPointByteCache256(in ReadOnlySpan<ECPoint256> points) {
            _points = new ECPoint256[TOTAL_POINTS_COUNT];
            _keyBytesCount = BigInteger256.BYTES_SIZE;
            for (var i = 0; i < TOTAL_POINTS_COUNT; i++) {
                _points[i] = points[i];
            }

        }

        public ECPoint256 Get(int byteIndex, byte byteValue) {
            if (byteIndex >= _keyBytesCount) {
                return _curve.Infinity;
            }
            return _points[byteIndex * 256 + byteValue];
        }

        public void SaveTo(Stream stream) {
            Span<byte> data = stackalloc byte[TOTAL_POINTS_BYTES];
            var ptr = 0;
            for (var i = 0; i < 256 / 8; i++) {
                for (var j = 0; j < 256; j++) {
                    var point = Get(i, (byte)j);
                    for (var k = 0; k < BigInteger256.BYTES_SIZE; k++) {
                        data[ptr++] = point.X.GetByte(k);
                    }
                    for (var k = 0; k < BigInteger256.BYTES_SIZE; k++) {
                        data[ptr++] = point.Y.GetByte(k);
                    }
                }
            }
            stream.Write(data);
        }

        public static ECPointByteCache256 ReadFrom(Stream stream, ECCurve256 curve) {
            var data = new byte[TOTAL_POINTS_BYTES];
            var points = new ECPoint256[TOTAL_POINTS_COUNT];
            stream.Read(data);
            var ptr = 0;
            var pi = 0;
            for (var i = 0; i < 256 / 8; i++) {
                for (var j = 0; j < 256; j++) {
                    //todo: faster way to copy
                    var x = new BigInteger256(0);
                    var y = new BigInteger256(0);
                    for (var k = 0; k < BigInteger256.BYTES_SIZE; k++) {
                        x.SetByte(k, data[ptr++]);
                    }
                    for (var k = 0; k < BigInteger256.BYTES_SIZE; k++) {
                        y.SetByte(k, data[ptr++]);
                    }
                    points[pi++] = new ECPoint256(x, y, curve);
                }
            }
            return new ECPointByteCache256(points);
        }

        public static ECPointByteCache256 ReadEmbeded(ECCurve256 curve) {
            var curveName = curve.Name;
            var resourceName = $"Ecc.EC256.{curveName}.cache.dat";
            using var resource = typeof(ECPointByteCache256).Assembly.GetManifestResourceStream(resourceName) ?? throw new Exception("Resource is not found");
            var cache = ReadFrom(resource, curve);
            return cache;
        }

        private static ECPointByteCache256? _secp256k1;
        public static ECPointByteCache256 Secp256k1 {
            get {
                return _secp256k1 ??= ReadEmbeded(ECCurve256.Secp256k1);
            }
        }

        private static ECPointByteCache256? _nistP256;
        public static ECPointByteCache256 NistP256 {
            get {
                return _nistP256 ??= ReadEmbeded(ECCurve256.NistP256);
            }
        }
    }
}
