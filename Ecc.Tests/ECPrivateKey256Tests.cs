﻿using System;
using System.Diagnostics;
using Ecc.Math;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ecc.Tests {
    [TestFixture]
    [TestOf(typeof(ECPrivateKey256))]
    public class ECPrivateKey256Tests {

        // [Test]
        // todo: low order works bad when generating new random
        // public void CreateTest() {
        //     var curve = new ECCurve256(name: null, a: 2, b: 3, modulus: 97, order: 100, cofactor: default, gx: default, gy: default);
        //     var privateKey = ECPrivateKey256.Create(curve);
        //     ClassicAssert.IsNotNull(privateKey.Curve);
        // }

        [Test]
        public void SignIntTest() {
            var curve = ECCurve256.Secp256k1;
            var privateKey = curve.CreatePrivateKey("8ce00ada2dffcfe03bd4775e90588f3f039bd83d56026fafd6f33080ebff72e8");
            var msg = BigInteger256.ParseHexUnsigned("7846e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
            var random = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var signature = privateKey.SignHash(msg, random)!;
            ClassicAssert.IsNotNull(signature, "signature must not be null");
            var rhex = signature.Value.R.ToHexUnsigned();
            var shex = signature.Value.S.ToHexUnsigned();
            ClassicAssert.AreEqual("2794dd08b1dfa958552bc37916515a3accb0527e40f9291d62cc4316047d24dd", rhex);
            ClassicAssert.AreEqual("5dd1f95f962bb6871967dc17b22217100daa00a3756feb1e16be3e6936fd8594", shex);
            ClassicAssert.AreEqual("2794dd08b1dfa958552bc37916515a3accb0527e40f9291d62cc4316047d24dd5dd1f95f962bb6871967dc17b22217100daa00a3756feb1e16be3e6936fd8594", signature.Value.ToHexString());
        }

        [Test]
        public void SignBytesTest() {
            var curve = ECCurve256.Secp256k1;
            var privateKey = curve.CreatePrivateKey("8ce00ada2dffcfe03bd4775e90588f3f039bd83d56026fafd6f33080ebff72e8");
            var msgHex = "7846e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715";
            var msg = GetBytes(msgHex);
            var random = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var signature = privateKey.Sign(msg, random)!;
            ClassicAssert.IsNotNull(signature, "signature must not be null");
            var rhex = signature.Value.R.ToHexUnsigned();
            var shex = signature.Value.S.ToHexUnsigned();
            ClassicAssert.AreEqual("2794dd08b1dfa958552bc37916515a3accb0527e40f9291d62cc4316047d24dd", rhex);
            ClassicAssert.AreEqual("5dd1f95f962bb6871967dc17b22217100daa00a3756feb1e16be3e6936fd8594", shex);
            ClassicAssert.AreEqual("2794dd08b1dfa958552bc37916515a3accb0527e40f9291d62cc4316047d24dd5dd1f95f962bb6871967dc17b22217100daa00a3756feb1e16be3e6936fd8594", signature.Value.ToHexString());
        }

        [Test]
        public void SignVerifyTest() {
            foreach (var curve in ECCurve256.GetNamedCurves()) {
                var privateKey = curve.CreateKeyPair();
                var msg = BigInteger256.ParseHexUnsigned("7846e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");
                var signature = privateKey.SignHash(msg);
                var valid = privateKey.PublicKey.VerifySignature(msg, signature);
                Assert.That(valid, Is.True, $"curve {curve.Name}, r and s are valid");
                var badSignature = new ECSignature256(
                    signature.R.ModAdd(new BigInteger256(5), signature.Curve.Modulus),
                    signature.S,
                    signature.Curve
                );
                valid = privateKey.PublicKey.VerifySignature(msg, badSignature);
                Assert.That(valid, Is.False, $"curve {curve.Name}, r is invalid");
                badSignature = new ECSignature256(
                    signature.R,
                    signature.S.ModAdd(new BigInteger256(5), signature.Curve.Modulus),
                    signature.Curve
                );
                valid = privateKey.PublicKey.VerifySignature(msg, badSignature);
                Assert.That(valid, Is.False, $"curve {curve.Name}, s is invalid");

                var badHash = msg + msg;
                valid = privateKey.PublicKey.VerifySignature(badHash, signature);
                Assert.That(valid, Is.False, $"curve {curve.Name}, s is invalid");
            }
        }

        [Test]
        public void SignaturePerformanceTest() {
            const int count = 10;
            var curve = ECCurve256.Secp256k1;
            var privateKey = curve.CreatePrivateKey("8ce00ada2dffcfe03bd4775e90588f3f039bd83d56026fafd6f33080ebff72e8");
            var random = BigInteger256.ParseHexUnsigned("cd6f06360fa5af8415f7a678ab45d8c1d435f8cf054b0f5902237e8cb9ee5fe5");
            var msg = BigInteger256.ParseHexUnsigned("7846e3be8abd2e089ed812475be9b51c3cfcc1a04fafa2ddb6ca6869bf272715");

            var watch = new Stopwatch();
            watch.Start();
            for (var i = 0; i < count; i++) {
                var _ = privateKey.SignHash(msg, random);
            }
            watch.Stop();
            var performance = System.Math.Round(1000 * count / watch.Elapsed.TotalMilliseconds, 3);
            Console.WriteLine($"ec256 {performance} signatures per second");
        }


        [TestCase("42c6d7b9570c75e3778b3ac7bfd851198816da1dd39ce420f16378c9418b8a58", "0320319526ae7bb161eb650486c9c4ff80ab4f9e18d04d9da3651000d0ac335f16")]
        [TestCase("754119b222e208b4c24936bd6aae22b3f760956f905103270705e5257e467344", "02334180cfb8553b774d871c36be174171003cd13cb8325ad091b4f4b10934662f")]
        [TestCase("9ada5ef64bff005b95afb0acb5f7d51df1f42ed5435c09ab7eb4ed9b6eb08572", "02861529d088817d897efcc7233ff344a85c905bd2ae524b359eacd39537b5cb8e")]
        public void PublicKeyTest(string privateKeyHex, string publicKeyHex) {
            var curve = ECCurve256.Secp256k1;
            var privateKey = curve.CreatePrivateKey(privateKeyHex);
            var publicKey = privateKey.PublicKey;
            ClassicAssert.AreEqual(publicKeyHex, publicKey.ToString());
        }

        private static byte[] GetBytes(ReadOnlySpan<char> hex) {
            Span<byte> bytes = stackalloc byte[hex.GetHexBytesCount()];
            hex.ToBytesFromHex(bytes);
            return bytes.ToArray();
        }

    }
}
