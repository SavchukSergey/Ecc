using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger512 {

        public readonly int Compare(in BigInteger512 other) {
            for (var i = UINT64_SIZE - 1; i >= 0; i--) {
                var leftBt = UInt64[i];
                var rightBt = other.UInt64[i];
                if (leftBt > rightBt) {
                    return 1;
                }
                if (leftBt < rightBt) {
                    return -1;
                }
            }
            return 0;
        }


        public static bool Equals(in BigInteger512 left, in BigInteger512 right) {
            return Compare(left, right) == 0;
        }

        public readonly override bool Equals(object? other) {
            if (other is BigInteger512 val) {
                return Equals(this, val);
            }
            return false;
        }

        #region Compare to BigInteger512

        public static int Compare(in BigInteger512 left, in BigInteger512 right) {
            for (var i = UINT64_SIZE - 1; i >= 0; i--) {
                var leftBt = left.UInt64[i];
                var rightBt = right.UInt64[i];
                if (leftBt > rightBt) {
                    return 1;
                }
                if (leftBt < rightBt) {
                    return -1;
                }
            }
            return 0;
        }

        public static bool operator <(in BigInteger512 left, in BigInteger512 right) {
            return Compare(left, right) < 0;
        }

        public static bool operator >(in BigInteger512 left, in BigInteger512 right) {
            return Compare(left, right) > 0;
        }

        public static bool operator <=(in BigInteger512 left, in BigInteger512 right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator >=(in BigInteger512 left, in BigInteger512 right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator ==(in BigInteger512 left, in BigInteger512 right) {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(in BigInteger512 left, in BigInteger512 right) {
            return Compare(left, right) != 0;
        }

        #endregion

        #region Compare to BigInteger256

        public static int Compare(in BigInteger512 left, in BigInteger256 right) {
            if (!left.BiHigh256.IsZero) {
                return 1;
            }
            return left.BiLow256.Compare(in right);
        }

        public static bool operator <(in BigInteger512 left, in BigInteger256 right) {
            return Compare(left, right) < 0;
        }

        public static bool operator >(in BigInteger512 left, in BigInteger256 right) {
            return Compare(left, right) > 0;
        }

        public static bool operator <=(in BigInteger512 left, in BigInteger256 right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator >=(in BigInteger512 left, in BigInteger256 right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator ==(in BigInteger512 left, in BigInteger256 right) {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(in BigInteger512 left, in BigInteger256 right) {
            return Compare(left, right) != 0;
        }

        #endregion


        #region Compare to BigInteger192

        public static int Compare(in BigInteger512 left, in BigInteger192 right) {
            if (left.UInt64[7] != 0) {
                return 1;
            }
            if (left.UInt64[6] != 0) {
                return 1;
            }
            if (left.UInt64[5] != 0) {
                return 1;
            }
            if (left.UInt64[4] != 0) {
                return 1;
            }
            if (left.UInt64[3] != 0) {
                return 1;
            }
            return BigInteger192.Compare(left.BiLow192, right);
        }

        public static bool operator <(in BigInteger512 left, in BigInteger192 right) {
            return Compare(left, right) < 0;
        }

        public static bool operator >(in BigInteger512 left, in BigInteger192 right) {
            return Compare(left, right) > 0;
        }

        public static bool operator <=(in BigInteger512 left, in BigInteger192 right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator >=(in BigInteger512 left, in BigInteger192 right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator ==(in BigInteger512 left, in BigInteger192 right) {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(in BigInteger512 left, in BigInteger192 right) {
            return Compare(left, right) != 0;
        }

        #endregion

        #region Compare to BigInteger128

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(in BigInteger512 left, in BigInteger128 right) {
            if (!left.BiHigh384.IsZero) {
                return 1;
            }

            return BigInteger128.Compare(left.BiLow128, right);
        }

        public static bool operator <(in BigInteger512 left, in BigInteger128 right) {
            return Compare(left, right) < 0;
        }

        public static bool operator >(in BigInteger512 left, in BigInteger128 right) {
            return Compare(left, right) > 0;
        }

        public static bool operator <=(in BigInteger512 left, in BigInteger128 right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator >=(in BigInteger512 left, in BigInteger128 right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator ==(in BigInteger512 left, in BigInteger128 right) {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(in BigInteger512 left, in BigInteger128 right) {
            return Compare(left, right) != 0;
        }

        #endregion

        #region Compare to UInt64

        public static int Compare(in BigInteger512 left, ulong right) {
            for (var i = UINT64_SIZE - 1; i >= 1; i--) {
                var leftBt = left.UInt64[i];
                if (leftBt > 0) {
                    return 1;
                }
            }
            var lastU64 = left.UInt64[UINT64_SIZE - 1];
            if (lastU64 > right) {
                return 1;
            }
            if (lastU64 < right) {
                return -1;
            }
            return 0;
        }

        public static bool operator <(in BigInteger512 left, ulong right) {
            return Compare(left, right) < 0;
        }

        public static bool operator >(in BigInteger512 left, ulong right) {
            return Compare(left, right) > 0;
        }

        public static bool operator >=(in BigInteger512 left, ulong right) {
            return Compare(left, right) >= 0;
        }

        public static bool operator <=(in BigInteger512 left, ulong right) {
            return Compare(left, right) <= 0;
        }

        public static bool operator ==(in BigInteger512 left, ulong right) {
            return Compare(left, right) == 0;
        }

        public static bool operator !=(in BigInteger512 left, ulong right) {
            return Compare(left, right) != 0;
        }

        #endregion

    }
}