# Usage

```c#
var curve = ECCurve.Secp256k1;
var keyPair = curve.CreateKeyPair();
var msg = new BigInteger(4579485729345);
var signature = keyPair.Sign(msg);
var valid = keyPair.PublicKey.VerifySignature(msg, signature);
```