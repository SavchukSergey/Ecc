using Ecc.Math;

namespace Ecc.EC256 {
    public abstract class BaseCachedCurve256 : ECCurve256 {

        protected BaseCachedCurve256(string name, in BigInteger256 a, in BigInteger256 b, in BigInteger256 modulus, in BigInteger256 order, in BigInteger256 cofactor, in BigInteger256 gx, in BigInteger256 gy) : base(name, a, b, modulus, order, cofactor, gx, gy) {
        }

        public sealed override void MulG(in BigInteger256 k, out ECPoint256 result) {
            var acc = new ECProjectiveMontgomeryPoint256(
                new BigInteger256(0),
                new BigInteger256(0),
                this
            );
            for (var i = 0; i < BigInteger256.BYTES_SIZE; i++) {
                var bt = k.GetByte(i);
                acc += Cache.Get(i, bt);
            }
            result = acc.Reduce().ToAffinePoint();
        }

        protected abstract ECProjectiveMontgomeryPointByteCache256 Cache { get; }

    }
}