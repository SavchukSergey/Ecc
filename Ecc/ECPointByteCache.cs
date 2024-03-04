namespace Ecc {
    public class ECPointByteCache {

        private readonly ECPoint[] _points;

        private readonly int _keyBytesCount;

        public ECPointByteCache(in ECPoint generator, int keySize) {
            var keyBytesCount = (keySize + 7) / 8;
            _keyBytesCount = keyBytesCount;
            _points = new ECPoint[keyBytesCount * 256];
            var shiftedGenerator = generator;

            for (var i = 0; i < keyBytesCount; i++) {
                var point = ECPoint.Infinity;
                for (var j = 0; j < 256; j++) {
                    _points[i * 256 + j] = point;
                    point = point + shiftedGenerator;
                }
                shiftedGenerator = point;
            }

        }

        public ref readonly ECPoint Get(int byteIndex, byte byteValue) {
            if (byteIndex >= _keyBytesCount) {
                return ref ECPoint.Infinity;
            }
            return ref _points[byteIndex * 256 + byteValue];
        }

    }
}
