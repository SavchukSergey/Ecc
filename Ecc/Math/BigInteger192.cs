using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ecc.Math {
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    [SkipLocalsInit]
    public unsafe partial struct BigInteger192 {

        public const int BITS_SIZE = 192;
        public const int BYTES_SIZE = BITS_SIZE / 8;

        [FieldOffset(0)]
        public UInt128 LowUInt128;

        [FieldOffset(8)]
        public UInt128 HighUInt128;

        [FieldOffset(16)]
        public ulong HighUInt64;

        public static readonly BigInteger192 Zero = new();

    }
}
