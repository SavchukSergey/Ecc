namespace Ecc {
    public class ECPointBitCache {

        private readonly ECPoint[] _points;

        public ECPointBitCache(in ECPoint generator, int keySize) {
            _points = new ECPoint[keySize];
            var point = generator;
            for (var i = 0; i < keySize; i++) {
                _points[i] = point;
                point = point * 2;
            }
        }

        public ref readonly ECPoint Pow2(int exp) {
            return ref _points[exp];
        }

    }
}
