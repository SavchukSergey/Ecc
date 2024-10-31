using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public readonly struct MontgomeryContext256 {

        public readonly BigInteger256 Modulus;
        private readonly BigInteger512 _r;
        private readonly BigInteger256 _rm;
        private readonly BigInteger256 _beta;
        public readonly BigInteger256 One;

        public MontgomeryContext256(in BigInteger256 modulus) {
            // k is supposed to be 256
            // modulus >= 2 ^ 255, odd
            _r = new BigInteger512(new BigInteger256(0), new BigInteger256(1));
            Modulus = modulus;
            _beta = (_r - new BigInteger512(modulus).ModInverse(_r)).BiLow256;

            _rm = _r % Modulus;

            One = _rm;
        }

        public readonly BigInteger256 ToMontgomery(in BigInteger256 x) {
            return x.ModMul(_rm, Modulus);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly BigInteger256 ModPow(in BigInteger256 value, in BigInteger256 exp) {
            ModPow(in value, in exp, out var result);
            return result;
        }

        public readonly void ModPow(in BigInteger256 value, in BigInteger256 exp, out BigInteger256 result) {
            result = One;
            var walker = value;
            for (var bit = 0; bit < BigInteger256.BITS_SIZE; bit++) {
                if (exp.GetBit(bit)) {
                    result = ModMul(result, walker);
                }
                walker = ModSquare(walker);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly BigInteger256 ModMul(in BigInteger256 u, in BigInteger256 v) {
            ModMul(in u, in v, out var result);
            return result;
        }

        public readonly void ModMul(in BigInteger256 u, in BigInteger256 v, out BigInteger256 result) {
            BigInteger256.Mul(in u, in v, out var temp);
            Reduce(in temp, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly BigInteger256 ModSquare(in BigInteger256 u) {
            ModSquare(in u, out var result);
            return result;
        }

        public readonly void ModSquare(in BigInteger256 u, out BigInteger256 result) {
            u.Square(out var sq);
            Reduce(sq, out result);
        }

        public readonly BigInteger256 ModCube(in BigInteger256 u) {
            ModSquare(in u, out var u2);
            return ModMul(u, u2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly BigInteger256 Reduce(in BigInteger512 x) {
            Reduce(x, out var result);
            return result;
        }

        public readonly void Reduce(in BigInteger512 x, out BigInteger256 result) {
            BigInteger256.MulLow256(in x.BiLow256, _beta, out var s2);
            BigInteger256.Mul(in Modulus, in s2, out var s3);
            // x + s3 requires extra one bit precission
            var carry = s3.AssignAdd(x);
            if (carry || s3.BiHigh256 >= Modulus) {
                s3.BiHigh256.AssignSub(in Modulus);
            }
            result = s3.BiHigh256;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly BigInteger256 Reduce(in BigInteger256 x) {
            Reduce(in x, out var result);
            return result;
        }

        public readonly void Reduce(in BigInteger256 x, out BigInteger256 result) {
            BigInteger256.MulLow256(x, _beta, out var s2);
            BigInteger256.Mul(in Modulus, in s2, out var s3);

            // x + s3 requires extra one bit precission
            var carry = s3.AssignAdd(x);
            if (carry || s3.BiHigh256 >= Modulus) {
                s3.BiHigh256.AssignSub(in Modulus);
            }
            result = s3.BiHigh256;
        }

        public readonly BigInteger256 ModSub(in BigInteger256 left, in BigInteger256 right) {
            return left.ModSub(in right, in Modulus);
        }

        public readonly BigInteger256 ModAdd(in BigInteger256 left, in BigInteger256 right) {
            return left.ModAdd(in right, in Modulus);
        }

        public readonly BigInteger256 ModTripple(in BigInteger256 value) {
            return value.ModTriple(in Modulus);
        }

        public readonly BigInteger256 ModDouble(in BigInteger256 value) {
            return value.ModDouble(in Modulus);
        }

        public bool IsValid {
            get {
                var gcd = BigInteger512.Gcd(_r, Modulus);
                if (!gcd.IsOne) {
                    return false;
                }

                //r * rInv - m * beta = 1

                var rInv = _r.ModInverse(new BigInteger512(Modulus));
                return ((_r * rInv) - (Modulus * _beta)).IsOne;
            }
        }
    }
}
