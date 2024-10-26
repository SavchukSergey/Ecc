using System;
using System.Runtime.CompilerServices;

namespace Ecc.Math {
    public unsafe partial struct BigInteger192 {

        public static void DivRem(in BigInteger192 dividend, in BigInteger192 divisor, out BigInteger192 quotient, out BigInteger192 remainder) {
            if (divisor.UInt64[2] == 0) {
                if (divisor.UInt64[1] == 0) {
                    DivRem(in dividend, divisor.LowUInt64, out quotient, out remainder);
                    return;
                } else {
                    DivRem(in dividend, in divisor.BiLow128, out quotient, out remainder);
                    return;
                }
            }

            //todo: check is dividendLZC - divisorLZC < 64 then call SingleShot
            DivRemGuessSingleShot(in dividend, in divisor, out var quotientSmall, out remainder);
            quotient = new BigInteger192(
                quotientSmall
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger192 dividend, in BigInteger128 divisor, out BigInteger192 quotient, out BigInteger128 remainder) {
            if (divisor.UInt64[1] == 0) {
                DivRem(in dividend, divisor.LowUInt64, out quotient, out remainder);
                return;
            }
            DivRemGuess(in dividend, in divisor, out var quotientSmall, out remainder);
            quotient = new BigInteger192(quotientSmall);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger192 dividend, in BigInteger128 divisor, out BigInteger192 quotient, out BigInteger192 remainder) {
            if (dividend.HighUInt64 == 0) {
                BigInteger128.DivRem(in dividend.BiLow128, in divisor, out var quotientSmall, out var remainderSmall);
                remainder = new BigInteger192(remainderSmall);
                quotient = new BigInteger192(quotientSmall);
                return;
            }

            DivRem(in dividend, in divisor, out quotient, out BigInteger128 remainder128);
            remainder = new BigInteger192(remainder128);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger192 dividend, ulong divisor, out BigInteger192 quotient, out BigInteger192 remainder) {
            DivRem(in dividend, divisor, out quotient, out ulong remainder64);
            remainder = new BigInteger192(remainder64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger192 dividend, ulong divisor, out BigInteger192 quotient, out BigInteger128 remainder) {
            DivRem(in dividend, divisor, out quotient, out ulong remainder64);
            remainder = new BigInteger128(remainder64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger192 dividend, uint divisor, out BigInteger192 quotient, out BigInteger192 remainder) {
            DivRem(in dividend, divisor, out quotient, out uint remainder32);
            remainder = new BigInteger192(remainder32);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DivRem(in BigInteger192 dividend, uint divisor, out BigInteger192 quotient, out BigInteger128 remainder) {
            DivRem(in dividend, divisor, out quotient, out uint remainder32);
            remainder = new BigInteger128(remainder32);
        }

        public static void DivRem(in BigInteger192 dividend, ulong divisor, out BigInteger192 quotient, out ulong remainder) {
            if (divisor <= uint.MaxValue) {
                DivRem(dividend, (uint)divisor, out quotient, out uint remainder32);
                remainder = remainder32;
                return;
            }

            if (System.Runtime.Intrinsics.X86.X86Base.X64.IsSupported) {
                var (q2, r2) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[2], 0, divisor);
                var (q1, r1) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[1], r2, divisor);
                (var q0, remainder) = System.Runtime.Intrinsics.X86.X86Base.X64.DivRem(dividend.UInt64[0], r1, divisor);
                quotient = new BigInteger192(q0, q1, q2);
            } else {
                quotient = new BigInteger192();
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

        public static void DivRem(in BigInteger192 dividend, uint divisor, out BigInteger192 quotient, out uint remainder) {
            quotient = new BigInteger192();
            var rem = 0ul;
            for (var i = UINT32_SIZE - 1; i >= 0; i--) {
                var partialDividend = (rem << 32) + dividend.UInt32[i];
                var (partialQuotient, partialRemainder) = ulong.DivRem(partialDividend, divisor);
                quotient.UInt32[i] = (uint)partialQuotient;
                rem = partialRemainder;
            }
            remainder = (uint)rem;
        }

        public static BigInteger192 operator /(in BigInteger192 left, in BigInteger192 right) {
            DivRem(left, right, out var quotient, out var _);
            return quotient;
        }

        public static BigInteger192 operator %(in BigInteger192 left, in BigInteger192 right) {
            DivRem(left, right, out var _, out var remainder);
            return remainder;
        }

    }
}