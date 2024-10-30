namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static BezoutIdentity256 EuclidExtended(in BigInteger256 a, in BigInteger256 b) {
            var s0 = new BigInteger256(1);
            var t0 = new BigInteger256(0);
            var s1 = new BigInteger256(0);
            var t1 = new BigInteger256(1);
            var r0 = a;
            var r1 = b;

            //https://stackoverflow.com/questions/67097428/is-it-possible-to-implement-the-extended-euclidean-algorithm-with-unsigned-machi
            var cnt = false;
            while (!r1.IsZero) {
                DivRem(in r0, in r1, out var quotient, out var r2);
                MulLow256(in quotient, in s1, out var temp1);
                var s2 = s0 + temp1;
                MulLow256(in quotient, in t1, out var temp2);
                var t2 = t0 + temp2;

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
            return new BezoutIdentity256 {
                A = a,
                B = b,
                X = s0,
                Y = t0
            };
        }

        public static BigInteger256 EuclidExtendedX(in BigInteger256 a, in BigInteger256 b) {
            var s0 = new BigInteger256(1);
            var s1 = new BigInteger256(0);
            var r0 = a;
            var r1 = b;

            //https://stackoverflow.com/questions/67097428/is-it-possible-to-implement-the-extended-euclidean-algorithm-with-unsigned-machi
            var cnt = false;
            while (!r1.IsZero) {
                DivRem(in r0, in r1, out var quotient, out var r2);
                MulLow256(in quotient, in s1, out var temp);
                var s2 = s0 + temp;

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