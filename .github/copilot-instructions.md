# Warbound Code Style Guide for GitHub Copilot

When generating code for this repository, please follow these style rules:

---

## Indentation & Formatting

- Use **spaces** for indentation — 4 spaces per level.
- Only 1 blank line is allowed between members.
- Always insert a newline at the end of every file.

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

## ACEP: Agentic Context & Execution Pipeline
In order to operate as an agent in our ACEP codebase follow these steps:

1. **Ingest the GitHub Issue**  
   - Every GitHub Issue in this pipeline contains **only** a link to the real action to be taken on a PLAN (e.g. `/ACEP/Plans/______.md`).  
   - Follow that link and fully read the issue's actual requirements and implementation details. This is your source of truth and ACEP PLAN to be completed.

2. **Load Context Profiles**  
   - Before opening or editing **any** code file, look for ACEP files:  
     - **File‑level**: `X.Agent.md` sitting next to `X.cs`
        - Example: Core/Logs/Logging.cs is accompanied by `Core/Logs/Logging.Agent.md`
     - **Namespace‑level**: `__Namespace.Agent.md` in the same folder
        - Example: Core/ETL has a file named Core/ETL/_ETL.Agent.md as an overview for that namespace.
   - These files contain context about how to use the code, conventions, and patterns that must be followed.
   - If there is confusion read all `X.Agent.md` files in the directory to make an educated decision.

3. **Editing Context Profiles**
   - When working on an *ACEP Plan* you must:
     - Review the Plan and understand the ethos and purpose behind the changes to the codebase.
     - Modify and update the ACEP markdown files (if they exist) (do not create .md files unless the Plan explicitly asks for it) for the class or the namespace to reflect the changes you are making.
     - If there is a follow up Plan to be written. Only write it after all of the current Plans's code is complete and tested and ready for it's pull request.

4. **Testing**
   - When writing tests do not write any integration tests or mocking unless explicitly specified in the Plan text.
