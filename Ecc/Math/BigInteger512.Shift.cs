namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public void AssignLeftShiftQuarter() {
            UInt64[7] = UInt64[5];
            UInt64[6] = UInt64[4];
            UInt64[5] = UInt64[3];
            UInt64[4] = UInt64[2];
            UInt64[3] = UInt64[1];
            UInt64[2] = UInt64[0];
            UInt64[1] = 0;
            UInt64[0] = 0;
        }

        public void AssignLeftShiftHalf() {
            High = Low;
            Low = new BigInteger256(0);
        }

        public readonly BigInteger512 LeftShiftHalf() {
            return new BigInteger512(new BigInteger256(0), Low);
        }

    }
}