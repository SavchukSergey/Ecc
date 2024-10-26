namespace Ecc.Math {
    public unsafe partial struct BigInteger128 {

        public static BigInteger128 EuclidExtendedX(in BigInteger128 a, in BigInteger128 b) {
            var s0 = new BigInteger128(1);
            var s1 = new BigInteger128(0);
            var r0 = a;
            var r1 = b;

            //https://stackoverflow.com/questions/67097428/is-it-possible-to-implement-the-extended-euclidean-algorithm-with-unsigned-machi
            var cnt = false;
            while (!r1.IsZero) {
                DivRem(in r0, in r1, out var quotient, out var r2);
                var s2 = s0 + MulLow128(in quotient, in s1);

                s0 = s1;
                s1 = s2;
                r0 = r1;
                r1 = r2;

                cnt = !cnt;
            }
            if (cnt) {
                s0 = b - s0;
            }

            return s0;
        }
    }
}