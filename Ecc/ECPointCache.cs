namespace Ecc {
    public class ECPointCache {

        private readonly ECPoint[] _points;

        public ECPointCache(in ECPoint generator, int keySize) {
            _points = new ECPoint[keySize];
            var point = generator;
            for (var i = 0; i < keySize; i++) {
                _points[i] = point;
                point = point * 2;
            }
        }

        public ref ECPoint Pow2(int exp) {
            return ref _points[exp];
        }

    }
}
