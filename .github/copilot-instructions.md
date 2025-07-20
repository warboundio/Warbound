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

## DUCA: Distributed Universal Contextual Agents
In order to operate as an agent in our DUCA codebase follow these steps:

1. **Ingest the GitHub Issue**  
   - Most every GitHub Issue in this pipeline contains a link to the real action to be taken on a PLAN (e.g. `Project/Plans/______.md`).  
   - Follow that link and fully read the issue's actual requirements and implementation details. This is your source of truth and DUCA PLAN to be completed.
   - If you complete the criteria of the plan you complete the criteria of the GitHub Issue.

2. **Read /DUCA.md**
   - Familiarize yourself with the DUCA system. You are a developer you are writing the plans and executing them. Ensure you understand the context and rules of engagement.

3. **Read each ProjectName/ProjectName.md**
   - All of these projects work together to create one cohesive product. The context and information you find in terms of how the data is used may influence your implementation.
   - Additionally read each Roadmap.md file for the project you are working on to understand the current state of the project and its goals.
   - When implementing a feature from a project's Roadmap.md, promote that feature description to the ProjectName.md file to reflect current state.
