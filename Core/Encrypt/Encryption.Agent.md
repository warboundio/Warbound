# Warbound Pattern: Encryption

This document defines the standard encryption usage pattern for all agents working in the Warbound codebase.

You do **not** need to know the internal implementation or cryptographic details. Just follow the pattern to **encrypt** and **decrypt** sensitive data.

---

## Pattern Summary

- Use `Encryption.Instance.Encrypt()` to encrypt a string
- Use `Encryption.Instance.Decrypt()` to decrypt a string
- **Never instantiate `Encryption` directly** — only tests are allowed to do that
- All encryption is handled using a secure key stored in an environment variable (`WARBOUND`)

---

## Example Usage

### Encrypting a string
```csharp
string encrypted = Encryption.Instance.Encrypt("my secret");
```

### Decrypting a string
```csharp
string plain = Encryption.Instance.Decrypt(encrypted);
```

---

## Rules

- ✅ Always use `Encryption.Instance`
- ✅ All encrypted strings are base64-encoded and safe for storage
- ✅ Use this pattern for anything sensitive (e.g. tokens, credentials)

- ❌ Do **not** create a new `Encryption()` instance
- ❌ Do **not** supply your own key
- ❌ Do **not** attempt to bypass or reimplement encryption manually

---

## Test-Only Exception

Tests may create a direct instance:
```csharp
Encryption testEncryption = new("base64key");
```
This is the **only** time you are allowed to manually construct the class.

---

## Why This Matters

This ensures:
- Centralized key handling (via environment)
- Strong, uniform AES-256-GCM encryption
- Safety for config files and persisted secrets

---

## Final Rule

👉 **Use `Encryption.Instance.Encrypt()` and `.Decrypt()`. Never new up the class. Never handle your own keys. The system handles everything.**
