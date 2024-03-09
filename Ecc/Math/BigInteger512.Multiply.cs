namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public static BigInteger1024 operator *(in BigInteger512 left, in BigInteger512 right) {
            var ah = left.High;
            var al = left.Low;
            var bh = right.High;
            var bl = right.Low;

            var zero = new BigInteger512(0);
            var x0 = new BigInteger1024(al * bl, zero);
            var x1 = new BigInteger1024(al * bh, zero) + new BigInteger1024(ah * bl, zero);
            x1.AssignLeftShiftQuarter();
            var x2 = new BigInteger1024(zero, ah * bh);

            return x0 + x1 + x2;
        }

        /// <summary>
        /// Multiplies two 512-bit numbers and returns first 512 bits of result
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static BigInteger512 MulLow(in BigInteger512 left, in BigInteger512 right) {
            var x0 = left.Low * right.Low;
            x0.AssignAddHigh(BigInteger256.MulLow(left.Low, right.High) + BigInteger256.MulLow(left.High, right.Low));

            return x0;
        }

    }
}