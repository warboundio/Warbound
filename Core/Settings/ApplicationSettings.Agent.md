# Warbound Pattern: Application Settings Access

This document defines how agents should **access and declare configuration values** in the Warbound codebase via the centralized `ApplicationSettings` class.

All secrets, tokens, API keys, and connection strings should be assumed to live in this config. Agents **must not** hardcode any secrets, reference environment variables, or manage raw file paths. You also **must not call** `.Save()` or `.Load()` under any circumstances.

---

## Pattern Summary

- All settings are accessed via `ApplicationSettings.Instance.PropertyName`
- The settings are **lazy-loaded**, encrypted, and cached automatically
- If a required setting is missing or blank, assume it will be populated later
- You **may** add new properties as needed
- You **must not** call `Save()`, `Load()`, or change any internal logic of the class

---

## Example Usage

### Reading a value:

```csharp
string token = ApplicationSettings.Instance.DiscordWarboundToken;
```

### Adding a new property:

In `ApplicationSettings.cs`, add:

```csharp
public string MyNewSecret { get; set; } = string.Empty;
```

Then access it like:

```csharp
string secret = ApplicationSettings.Instance.MyNewSecret;
```

Assume it will be filled in later — do not add fallbacks.

---

## Key Properties (non-exhaustive)

- `PostgresConnection` – Full connection string to the Postgres database
- `DiscordWarboundToken` – Bot token for Discord
- `BattleNetClientId` – Blizzard API client ID
- `BattleNetSecretId` – Blizzard API secret

---

## Do Not

- ❌ Do **not** read config from `appsettings.json`, `.env`, or any other file
- ❌ Do **not** use `Environment.GetEnvironmentVariable()`
- ❌ Do **not** inline secret strings or tokens
- ❌ Do **not** call `.Save()` or `.Load()`
- ❌ Do **not** modify the `ApplicationSettings` logic or constructor

---

## Why This Matters

- Ensures a **single source of truth** for all configuration data
- Supports **encrypted, secure storage** of credentials
- Allows seamless switching between local, CI, and production environments
- Maintains agent compatibility without brittle config logic

---

## Internal Mechanics (for awareness only — not for agent use)

- The encrypted config file is named `config.data`
- It is stored relative to the solution root and located automatically
- The loading process walks up from `AppContext.BaseDirectory` to find a `.csproj`, then uses the parent folder

Agents do **not** need to handle this. Just use the class.

---

## Final Rule

👉 **Use `ApplicationSettings.Instance`. Add properties if needed. Do not call `Save()`, `Load()`, or modify internal logic. Let the system handle the rest.**
