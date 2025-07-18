# Warbound Pattern: Logging

This document defines how all logging should be performed within the Warbound codebase. Logging is built on **Serilog**, but it is extended and centralized through a custom static `Logging` class.

Agents must never call Serilog directly. All log messages must go through the `Core.Logging` class.

---

## Pattern Summary

- Use `Logging.Info()`, `Logging.Warn()`, `Logging.Error()`, or `Logging.Fatal()`
- Do **not** use `Console.WriteLine()`
- Do **not** call Serilog APIs directly (`Log.Information`, etc.)
- Always pass a `classOrApplication` prefix to indicate log context — typically the class name
- All warnings, errors, and fatals are automatically sent to Discord

---

## Example Usage

### Informational
```csharp
Logging.Info(nameof(MyService), "Polling complete.");
```

### Warning
```csharp
Logging.Warn(nameof(TokenRefresher), "Token was expired; attempting refresh.");
```

### Error with Exception
```csharp
try
{
    // some risky code
}
catch (Exception ex)
{
    Logging.Error(nameof(MyWorker), "Failed during step execution", ex);
}
```

### Fatal
```csharp
Logging.Fatal(nameof(MainProgram), "Unexpected crash during startup", ex);
```

---

## Method Reference

```csharp
public static void Info(string classOrApplication, string message);
public static void Warn(string classOrApplication, string message);
public static void Error(string classOrApplication, string message);
public static void Error(string classOrApplication, string message, Exception ex);
public static void Fatal(string classOrApplication, string message, Exception ex);
```

---

## Important Guidelines

- ✅ Always log caught exceptions using `.Error()` or `.Fatal()`
- ✅ Always pass a meaningful `classOrApplication` identifier
- ✅ Treat warnings as a signal that recovery was needed (e.g. token expired)

- ❌ Do **not** use `Console.WriteLine()`
- ❌ Do **not** call Serilog APIs like `Log.Information()` directly
- ❌ Do **not** ignore exceptions silently

---

## Why This Matters

- Centralized logging lets us:
  - Send critical logs to Discord
  - Track logs across services with consistent format
  - Avoid logging inconsistencies

---

## Final Rule

👉 **Use the `Logging` class for all logs. Do not use Serilog directly. Never use Console.WriteLine. Always log exceptions.**
