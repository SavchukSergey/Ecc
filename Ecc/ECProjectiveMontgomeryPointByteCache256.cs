using System;
using System.IO;
using Ecc.Math;

namespace Ecc {
    public class ECProjectiveMontgomeryPointByteCache256 {

        private readonly ECProjectiveMontgomeryPoint256[] _points;

        private readonly int _keyBytesCount;

        public readonly ECCurve256 Curve;

        public const int TOTAL_POINTS_COUNT = 256 * 256 / 8; // 8192 points
        public const int POINT_DATA_SIZE = 2 * 256 / 8;      // 64 bytes: two of three coords (32 bytes each) for each point
        public const int TOTAL_POINTS_BYTES = TOTAL_POINTS_COUNT * POINT_DATA_SIZE; //0.5Mb

        public ECProjectiveMontgomeryPointByteCache256(ECPointByteCache256 inner) {
            Curve = inner.Curve;
            _keyBytesCount = 32;
            _points = new ECProjectiveMontgomeryPoint256[TOTAL_POINTS_COUNT];

            for (var byteIndex = 0; byteIndex < 32; byteIndex++) {
                for (var byteValue = 0; byteValue < 256; byteValue++) {
                    var point = inner.Get(byteIndex, (byte)byteValue);
                    _points[byteIndex * 256 + byteValue] = point.ToProjective().ToMontgomery();
                }
            }
        }

        private ECProjectiveMontgomeryPointByteCache256(in ReadOnlySpan<ECProjectiveMontgomeryPoint256> points, ECCurve256 curve) {
            Curve = curve;
            _points = new ECProjectiveMontgomeryPoint256[TOTAL_POINTS_COUNT];
            _keyBytesCount = BigInteger256.BYTES_SIZE;
            for (var i = 0; i < TOTAL_POINTS_COUNT; i++) {
                _points[i] = points[i];
            }
        }

        public ref readonly ECProjectiveMontgomeryPoint256 Get(int byteIndex, byte byteValue) {
            if (byteIndex >= _keyBytesCount) {
                throw new IndexOutOfRangeException();
            }
            return ref _points[byteIndex * 256 + byteValue];
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

        public static ECProjectiveMontgomeryPointByteCache256 ReadFrom(Stream stream, ECCurve256 curve) {
            var data = new byte[TOTAL_POINTS_BYTES];
            var points = new ECProjectiveMontgomeryPoint256[TOTAL_POINTS_COUNT];
            var _ = stream.Read(data);
            var ptr = 0;
            var pi = 0;
            for (var i = 0; i < 256 / 8; i++) {
                for (var j = 0; j < 256; j++) {
                    var x = new BigInteger256(0);
                    var y = new BigInteger256(0);
                    for (var k = 0; k < BigInteger256.BYTES_SIZE; k++) {
                        x.SetByte(k, data[ptr++]);
                    }
                    for (var k = 0; k < BigInteger256.BYTES_SIZE; k++) {
                        y.SetByte(k, data[ptr++]);
                    }
                    points[pi++] = new ECProjectiveMontgomeryPoint256(x, y, curve.MontgomeryContext.One, curve);
                }
            }
            return new ECProjectiveMontgomeryPointByteCache256(points, curve);
        }

        public static ECProjectiveMontgomeryPointByteCache256 ReadEmbeded(ECCurve256 curve) {
            var curveName = curve.Name;
            var resourceName = $"Ecc.EC256.{curveName}.mp.cache.dat";
            using var resource = typeof(ECProjectiveMontgomeryPointByteCache256).Assembly.GetManifestResourceStream(resourceName) ?? throw new Exception("Resource is not found");
            var cache = ReadFrom(resource, curve);
            return cache;
        }

        private static ECProjectiveMontgomeryPointByteCache256? _secp256k1;
        public static ECProjectiveMontgomeryPointByteCache256 Secp256k1 {
            get {
                return _secp256k1 ??= ReadEmbeded(ECCurve256.Secp256k1);
            }
        }

        private static ECProjectiveMontgomeryPointByteCache256? _nistP256;
        public static ECProjectiveMontgomeryPointByteCache256 NistP256 {
            get {
                return _nistP256 ??= ReadEmbeded(ECCurve256.NistP256);
            }
        }
    }
}
