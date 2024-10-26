using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(in BigInteger192 left, in BigInteger192 right) {
            if (left.HighUInt128 > right.HighUInt128) {
                return 1;
            }
            if (left.HighUInt128 < right.HighUInt128) {
                return -1;
            }
            if (left.LowUInt64 > right.LowUInt64) {
                return 1;
            }
            if (left.LowUInt64 < right.LowUInt64) {
                return -1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(in BigInteger192 left, in BigInteger128 right) {
            if (left.HighUInt64 != 0) {
                return 1;
            }
            if (left.LowUInt128 > right.UInt128) {
                return 1;
            }
            if (left.LowUInt128 < right.UInt128) {
                return -1;
            }
            return 0;
        }

        public static bool operator <(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) < 0;
        }

        public static bool operator >(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) > 0;
        }

        public static bool operator <=(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator >=(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator ==(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) != 0;
        }


        public static bool operator <(in BigInteger192 left, in BigInteger128 right) {
            return Compare(left, right) < 0;
        }

        public static bool operator >(in BigInteger192 left, in BigInteger128 right) {
            return Compare(left, right) > 0;
        }

        public static bool operator <=(in BigInteger192 left, in BigInteger128 right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator >=(in BigInteger192 left, in BigInteger128 right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator ==(in BigInteger192 left, in BigInteger128 right) {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(in BigInteger192 left, in BigInteger128 right) {
            return Compare(left, right) != 0;
        }

        public static bool Equals(in BigInteger192 left, in BigInteger192 right) {
            return Compare(left, right) == 0;
        }

        public readonly override bool Equals(object? other) {
            if (other is BigInteger192 val) {
                return Equals(this, val);
            }
            return false;
        }

        public override int GetHashCode() {
            uint res = 0;
            for (var i = 1; i < UINT32_SIZE; i++) {
                res ^= UInt32[i];
            }
            return (int)res;
        }
    }
}