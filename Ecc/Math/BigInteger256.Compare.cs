namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public readonly int Compare(in BigInteger256 other) {
            var h = BigInteger128.Compare(BiHigh, other.BiHigh);
            if (h != 0) {
                return h;
            }
            return BigInteger128.Compare(BiLow, other.BiLow);
        }

        public static int Compare(in BigInteger256 left, in BigInteger256 right) {
            var h = BigInteger128.Compare(left.BiHigh, right.BiHigh);
            if (h != 0) {
                return h;
            }
            return BigInteger128.Compare(left.BiLow, right.BiLow);
        }

        public static int Compare(in BigInteger256 left, in BigInteger128 right) {
            if (!left.BiHigh.IsZero) {
                return 1;
            }

            return BigInteger128.Compare(left.BiLow, right);
        }

        public static int Compare(in BigInteger256 left, ulong right) {
            if (!left.BiHigh.IsZero) {
                return 1;
            }
            return BigInteger128.Compare(left.BiLow, right);
        }

        public static bool Equals(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) == 0;
        }

        public readonly override bool Equals(object? other) {
            if (other is BigInteger256 val) {
                return Equals(this, val);
            }
            return false;
        }

        public static bool operator <(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) < 0;
        }

        public static bool operator >(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) > 0;
        }

        public static bool operator <=(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator >=(in BigInteger256 left, ulong right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator <=(in BigInteger256 left, ulong right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator >=(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator ==(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) != 0;
        }

    }
}