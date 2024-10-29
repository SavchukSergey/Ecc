using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger256 {

        public static void DivRem(in BigInteger256 dividend, in BigInteger256 divisor, out BigInteger256 quotient, out BigInteger256 remainder) {
            if (divisor.UInt64[3] == 0) {
                if (divisor.UInt64[2] == 0) {
                    if (divisor.UInt64[1] == 0) {
                        DivRem(in dividend, divisor.LowUInt64, out quotient, out remainder);
                        return;
                    } else {
                        DivRem(in dividend, in divisor.BiLow128, out quotient, out remainder);
                        return;
                    }
                } else {
                    DivRem(in dividend, in divisor.BiLow192, out quotient, out remainder);
                    return;
                }
            }

            //todo: check is dividendLZC - divisorLZC < 64 then call SingleShot
            DivRemGuessSingleShot(in dividend, in divisor, out var quotientSmall, out remainder);
            quotient = new BigInteger256(quotientSmall);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger256 dividend, in BigInteger192 divisor, out BigInteger256 quotient, out BigInteger192 remainder) {
            if (divisor.UInt64[2] == 0) {
                if (divisor.UInt64[1] == 0) {
                    DivRem(in dividend, divisor.LowUInt64, out quotient, out remainder);
                    return;
                } else {
                    DivRem(in dividend, in divisor.BiLow128, out quotient, out remainder);
                    return;
                }
            }

            DivRemGuess(in dividend, in divisor, out var quotientSmall, out remainder);
            quotient = new BigInteger256(quotientSmall);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger256 dividend, in BigInteger128 divisor, out BigInteger256 quotient, out BigInteger128 remainder) {
            if (divisor.UInt64[1] == 0) {
                DivRem(in dividend, divisor.LowUInt64, out quotient, out remainder);
                return;
            }

            DivRemGuess(in dividend, in divisor, out var quotientSmall, out remainder);
            quotient = new BigInteger256(quotientSmall);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger256 dividend, in BigInteger192 divisor, out BigInteger256 quotient, out BigInteger256 remainder) {
            remainder = new BigInteger256();
            if (dividend.HighUInt64 == 0) {
                quotient = new BigInteger256();
                BigInteger192.DivRem(in dividend.BiLow192, in divisor, out quotient.BiLow192, out remainder.BiLow192);
                return;
            } else {
                DivRem(in dividend, in divisor, out quotient, out remainder.BiLow192);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger256 dividend, in BigInteger128 divisor, out BigInteger256 quotient, out BigInteger256 remainder) {
            remainder = new BigInteger256();
            DivRem(in dividend, in divisor, out quotient, out remainder.BiLow128);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger256 dividend, in BigInteger128 divisor, out BigInteger256 quotient, out BigInteger192 remainder) {
            remainder = new BigInteger192();
            DivRem(in dividend, divisor, out quotient, out remainder.BiLow128);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger256 dividend, ulong divisor, out BigInteger256 quotient, out BigInteger256 remainder) {
            remainder = new BigInteger256();
            DivRem(in dividend, divisor, out quotient, out remainder.LowUInt64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger256 dividend, ulong divisor, out BigInteger256 quotient, out BigInteger192 remainder) {
            remainder = new BigInteger192();
            DivRem(in dividend, divisor, out quotient, out remainder.LowUInt64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger256 dividend, ulong divisor, out BigInteger256 quotient, out BigInteger128 remainder) {
            remainder = new BigInteger128();
            DivRem(in dividend, divisor, out quotient, out remainder.LowUInt64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger256 dividend, uint divisor, out BigInteger256 quotient, out BigInteger256 remainder) {
            remainder = new BigInteger256();
            DivRem(in dividend, divisor, out quotient, out remainder.LowUInt32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger256 dividend, uint divisor, out BigInteger256 quotient, out BigInteger192 remainder) {
            remainder = new BigInteger192();
            DivRem(in dividend, divisor, out quotient, out remainder.LowUInt32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger256 dividend, uint divisor, out BigInteger256 quotient, out BigInteger128 remainder) {
            remainder = new BigInteger128();
            DivRem(in dividend, divisor, out quotient, out remainder.LowUInt32);
        }

        public static void DivRem(in BigInteger256 dividend, ulong divisor, out BigInteger256 quotient, out ulong remainder) {
            if (divisor <= uint.MaxValue) {
                DivRem(dividend, (uint)divisor, out quotient, out uint remainder32);
                remainder = remainder32;
                return;
            }

            if (System.Runtime.Intrinsics.X86.X86Base.X64.IsSupported) {
                var (q3, r3) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[3], 0, divisor);
                var (q2, r2) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[2], r3, divisor);
                var (q1, r1) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[1], r2, divisor);
                (var q0, remainder) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[0], r1, divisor);
                quotient = new BigInteger256(q0, q1, q2, q3);
            } else {
                quotient = new BigInteger256();
                var rem = new UInt128();
                for (var i = UINT64_SIZE - 1; i >= 0; i--) {
                    var partialDividend = (rem << 64) + dividend.UInt64[i];
                    var (partialQuotient, partialRemainder) = UInt128.DivRem(partialDividend, divisor);
                    quotient.UInt64[i] = (ulong)partialQuotient;
                    rem = partialRemainder;
                }
                remainder = (ulong)rem;
            }
        }

        public static void DivRem(in BigInteger256 dividend, uint divisor, out BigInteger256 quotient, out uint remainder) {
            quotient = new BigInteger256();
            var rem = 0ul;
            for (var i = UINT32_SIZE - 1; i >= 0; i--) {
                var partialDividend = (rem << 32) + dividend.UInt32[i];
                var (partialQuotient, partialRemainder) = ulong.DivRem(partialDividend, divisor);
                quotient.UInt32[i] = (uint)partialQuotient;
                rem = partialRemainder;
            }
            remainder = (uint)rem;
        }

        public static BigInteger256 operator /(in BigInteger256 left, in BigInteger256 right) {
            DivRem(left, right, out var quotient, out var _);
            return quotient;
        }

        public static BigInteger256 operator %(in BigInteger256 left, in BigInteger256 right) {
            DivRem(left, right, out var _, out var remainder);
            return remainder;
        }
    }
}