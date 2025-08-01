﻿# Warbound Code Style Guide for GitHub Copilot

For each file you plan to commit please review it top to bottom and ensure it meets our coding standards.
If you do not follow these conventions, your pull request will be rejected.

---

## Indentation & Formatting
- Use **spaces** for indentation — 4 spaces per level.
- Only 1 blank line is allowed between members.
- Always insert a newline at the end of every file.
- Do not have '      ' leading whitespace when a line has no code.

---

## Variable Declarations
- **Do not use `var`**. Always use explicit types like `User user = new();`.

---

## Object Instantiation
- Prefer inline `new()` syntax when the type is known: `User user = new();`.

---

## Braces & Blocks
- **Always use braces** `{}` for all conditionals and loops — even single-line.

---

## Expressions & Language Features

- Prefer expression-bodied **methods** and **properties** when appropriate.
- Use modern C# features:
  - `switch` expressions
  - null coalescing (`??`, `??=`)
  - `nameof()`
  - interpolated strings
  - file-scoped namespaces (`namespace Foo;`)
  - top-level statements
  - simplified interpolation (`$"{x}"` over `string.Format(...)`)

---

## Naming Conventions

- Private fields: `_camelCase`
- Constants: `UPPER_CASE`
- Local variables: `camelCase`
- Interfaces: `IPascalCase`
- Enums: `PascalCase`
- Async methods: must end in `Async`
- **Do not use underscores** in method names (e.g., `Add_Ints_ReturnsCorrectSum` is incorrect).
- Instead, use descriptive PascalCase like `AddIntsReturnsCorrectSum`.

---

## Documentation

- Test methods (`*Tests.cs`) do **not** require documentation.

---

## Code Analysis & Style Rules

- Do not mark methods as `static` unless needed (`CA1822` is disabled).
- Do not recommend sealing internal types unless performance is critical.
- Do not add unnecessary `using` statements — especially `using Xunit;`, which is globally imported.
- Uninitialized int fields should be set to -1

---

## Testing Style

When writing test methods:

- Method names must begin with `ItShould`
- Use descriptive PascalCase that clearly states behavior
- Avoid under_scores
- Do not include `using Xunit;` — it is globally imported
- The generated test must compile and conform to `.editorconfig` rules
- Do not use mocking or in memory databases unless explicitly specified in the github issue text.

**✅ Good example:**

```csharp
[Fact]
public void ItShouldAddTwoNumbers()
{
    Calculator calc = new();
    Assert.Equal(5, calc.Add(2, 3));
}

**❌ Bad example:**
```csharp
[Fact]
public void Add_Ints_ReturnsCorrectSum()
{
    var calc = new Calculator();
    Assert.Equal(5, calc.Add(2, 3));
}
```