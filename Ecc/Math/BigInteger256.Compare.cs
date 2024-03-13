namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public readonly int Compare(in BigInteger256 other) {
            if (High < other.High) {
                return -1;
            }
            if (High > other.High) {
                return 1;
            }
            if (Low < other.Low) {
                return -1;
            }
            if (Low > other.Low) {
                return 1;
            }
            return 0;
        }

        public static int Compare(in BigInteger256 left, in BigInteger256 right) {
            if (left.High < right.High) {
                return -1;
            }
            if (left.High > right.High) {
                return 1;
            }
            if (left.Low < right.Low) {
                return -1;
            }
            if (left.Low > right.Low) {
                return 1;
            }
            return 0;
        }

        public static int Compare(in BigInteger256 left, ulong right) {
            if (left.High > 0) {
                return -1;
            }
            if (left.Low < right) {
                return -1;
            }
            if (left.Low > right) {
                return 1;
            }
            return 0;
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