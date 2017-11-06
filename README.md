# Usage

```c#
var curve = ECCurve.Secp256k1;
var privateKey = curve.CreateKeyPair();
var msg = new BigInteger(4579485729345);
var signature = privateKey.Sign(msg);
var valid = privateKey.PublicKey.VerifySignature(msg, signature);
```