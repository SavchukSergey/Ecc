using Ecc.Math;
using NUnit.Framework;

namespace Ecc.Tests.Math {
    public partial class ExtractHigh128Tests {

        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 000, "cd6f06360fa5af8415f7a678ab45d8c1")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 004, "d6f06360fa5af8415f7a678ab45d8c1d")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 064, "15f7a678ab45d8c1d435f8cf054b0f59")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 068, "5f7a678ab45d8c1d435f8cf054b0f590")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 128, "d435f8cf054b0f5902237e8cb9ee5fe5")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 132, "435f8cf054b0f5902237e8cb9ee5fe50")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 192, "02237e8cb9ee5fe50000000000000000")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 196, "2237e8cb9ee5fe500000000000000000")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 256, "00000000000000000000000000000000")]
        public void ExtractHigh128Test(string source, int count, string expected) {
            var left = BigInteger256.ParseHexUnsigned(source);
            var res128 = left.ExtractHigh128(count);
            Assert.That(res128.ToHexUnsigned(), Is.EqualTo(expected));
            var res64 = left.ExtractHigh64(count);
            Assert.That(res64, Is.EqualTo(BigInteger128.ParseHexUnsigned(expected).HighUInt64), $"{count}");
        }

        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 000, "cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 004, "d6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe50")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 064, "15f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe50000000000000000")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 068, "5f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe500000000000000000")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 128, "d435f8cf054b0f5902237e8cb9ee5fe500000000000000000000000000000000")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 132, "435f8cf054b0f5902237e8cb9ee5fe5000000000000000000000000000000000")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 192, "02237e8cb9ee5fe5000000000000000000000000000000000000000000000000")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 196, "2237e8cb9ee5fe50000000000000000000000000000000000000000000000000")]
        [TestCase("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5", 256, "0000000000000000000000000000000000000000000000000000000000000000")]
        public void AssignLeftShiftTest(string source, int count, string expected) {
            var left = BigInteger256.ParseHexUnsigned(source);
            left.AssignLeftShift(count);
            Assert.That(left.ToHexUnsigned(), Is.EqualTo(expected), $"{count}");
        }

    }
}