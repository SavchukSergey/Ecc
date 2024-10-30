namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public static BezoutIdentity512 EuclidExtended(in BigInteger512 a, in BigInteger512 b) {
            var s0 = new BigInteger512(1);
            var t0 = new BigInteger512(0);
            var s1 = new BigInteger512(0);
            var t1 = new BigInteger512(1);
            var r0 = a;
            var r1 = b;

            //https://stackoverflow.com/questions/67097428/is-it-possible-to-implement-the-extended-euclidean-algorithm-with-unsigned-machi
            var cnt = false;
            while (!r1.IsZero) {
                DivRem(in r0, in r1, out var quotient, out var r2);
                var s2 = s0 + MulLow(in quotient, in s1);
                var t2 = t0 + MulLow(in quotient, in t1);

                s0 = s1;
                s1 = s2;
                t0 = t1;
                t1 = t2;
                r0 = r1;
                r1 = r2;

                cnt = !cnt;
            }
            if (cnt) {
                s0 = b - s0;
            } else {
                t0 = a - t0;
            }
            return new BezoutIdentity512 {
                A = a,
                B = b,
                X = s0,
                Y = t0
            };
        }

        public static BigInteger512 EuclidExtendedX(in BigInteger512 a, in BigInteger512 b) {
            var s0 = new BigInteger512(1);
            var s1 = new BigInteger512(0);
            var r0 = a;
            var r1 = b;

            //https://stackoverflow.com/questions/67097428/is-it-possible-to-implement-the-extended-euclidean-algorithm-with-unsigned-machi
            var cnt = false;
            while (!r1.IsZero) {
                DivRem(in r0, in r1, out var quotient, out var r2);
                var s2 = s0 + MulLow(in quotient, in s1);

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
