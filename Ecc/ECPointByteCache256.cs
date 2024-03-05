namespace Ecc {
    public class ECPointByteCache256 {

        private readonly ECPoint256[] _points;

        private readonly int _keyBytesCount;

        public ECPointByteCache256(in ECPoint256 generator, int keySize) {
            var keyBytesCount = (keySize + 7) / 8;
            _keyBytesCount = keyBytesCount;
            _points = new ECPoint256[keyBytesCount * 256];
            var shiftedGenerator = generator;

            for (var i = 0; i < keyBytesCount; i++) {
                var point = ECPoint256.Infinity;
                for (var j = 0; j < 256; j++) {
                    _points[i * 256 + j] = point;
                    point = point + shiftedGenerator;
                }
                shiftedGenerator = point;
            }

        }

        public ref readonly ECPoint256 Get(int byteIndex, byte byteValue) {
            if (byteIndex >= _keyBytesCount) {
                return ref ECPoint256.Infinity;
            }
            return ref _points[byteIndex * 256 + byteValue];
        }

    }
}
