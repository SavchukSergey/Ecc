using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Compare(in BigInteger256 other) {
            var h = BigInteger128.Compare(BiHigh128, other.BiHigh128);
            if (h != 0) {
                return h;
            }
            return BigInteger128.Compare(BiLow128, other.BiLow128);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(in BigInteger256 left, in BigInteger256 right) {
            if (left.HighUInt128 > right.HighUInt128) {
                return 1;
            }
            if (left.HighUInt128 < right.HighUInt128) {
                return -1;
            }
            if (left.LowUInt128 > right.LowUInt128) {
                return 1;
            }
            if (left.LowUInt128 < right.LowUInt128) {
                return -1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(in BigInteger256 left, in BigInteger192 right) {
            if (left.HighUInt64 != 0) {
                return 1;
            }

            return BigInteger192.Compare(left.BiLow192, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(in BigInteger256 left, in BigInteger128 right) {
            if (left.HighUInt128 != 0) {
                return 1;
            }

            return BigInteger128.Compare(left.BiLow128, right);
        }

        public static int Compare(in BigInteger256 left, ulong right) {
            if (!left.BiHigh192.IsZero) {
                return 1;
            }
            if (left.LowUInt64 > right) {
                return 1;
            }
            if (left.LowUInt64 < right) {
                return -1;
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

        public override int GetHashCode() {
            uint res = 0;
            for (var i = 1; i < UINT32_SIZE; i++) {
                res ^= UInt32[i];
            }
            return (int)res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) < 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) <= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(in BigInteger256 left, in BigInteger256 right) {
            return Compare(in left, in right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in BigInteger256 left, in BigInteger256 right) {
            return Compare(left, right) != 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(in BigInteger256 left, in BigInteger192 right) {
            return Compare(left, right) < 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(in BigInteger256 left, in BigInteger192 right) {
            return Compare(left, right) > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(in BigInteger256 left, in BigInteger192 right) {
            return Compare(left, right) <= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(in BigInteger256 left, in BigInteger192 right) {
            return Compare(left, right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in BigInteger256 left, in BigInteger192 right) {
            return Compare(left, right) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in BigInteger256 left, in BigInteger192 right) {
            return Compare(left, right) != 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(in BigInteger256 left, ulong right) {
            return Compare(left, right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(in BigInteger256 left, ulong right) {
            return Compare(left, right) <= 0;
        }

    }
}