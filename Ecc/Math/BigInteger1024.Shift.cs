namespace Ecc.Math {
    public unsafe partial struct BigInteger1024 {

        public void AssignLeftShiftQuarter() {
            UInt64[15] = UInt64[13];
            UInt64[14] = UInt64[12];
            UInt64[13] = UInt64[11];
            UInt64[12] = UInt64[10];
            UInt64[11] = UInt64[9];
            UInt64[10] = UInt64[8];
            UInt64[9] = UInt64[7];
            UInt64[8] = UInt64[6];

            UInt64[7] = UInt64[5];
            UInt64[6] = UInt64[4];
            UInt64[5] = UInt64[3];
            UInt64[4] = UInt64[2];
            UInt64[3] = UInt64[1];
            UInt64[2] = UInt64[0];
            UInt64[1] = 0;
            UInt64[0] = 0;
        }

    }
}