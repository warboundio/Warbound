# DUCA: Distributed Universal Contextual Agents

DUCA is the framework powering autonomous Copilot Agents in this codebase. It defines the structure, expectations, and context required for agents to reliably execute code changes based on well-scoped Plans.

This document provides guidance for both **contributors** (who write Plans and context files) and **agents** (who execute the Plans).

---

## 🔧 For Contributors

### What is a DUCA Plan?

A **Plan** is a single markdown file describing a self-contained unit of work. It includes everything an agent needs to:

* Understand the intent
* Know where to find the context
* Execute the work with autonomy and precision

**Examples:**

* Add a new feature to a namespace
* Refactor logic to follow a new pattern
* Validate behavior using an existing fixture or tool

Plans are created by the same developers who execute them. The goal is to distill **why** the change matters and point to **where** relevant logic or context lives — not to prescribe how to do it.

> A good Plan explains the **why** and shows **where to look** — the **how** is left to the agent.

### Where Do Plans Live?

All Plans live under `/DUCA/Plans/`. Issues should link directly to these.

### Context Lives in `*.Agent.md` Files

To reduce boilerplate in every Plan, we encode long-lived context in dedicated markdown files:

* `X.Agent.md` files live next to source files (file-level context)
* `_Namespace.Agent.md` files live in a folder (namespace-level context)

Examples:

* `Core/Logs/Logging.Agent.md` documents how to log correctly
* `Core/Security/Encryption.Agent.md` defines how encryption must be handled

> 🔍 **Whenever you use or change a file, check if there's a `X.Agent.md` file next to it.** That's where the rules live.

These context files are critical to the DUCA system. Contributors must update them when Plans introduce behavior changes or establish new expectations.

### Tips for Writing Good Plans

* **Focus on the why.** State the intent clearly and concisely.
* **Link to all relevant `*.Agent.md` files.** Help agents locate context.
* **Define boundaries.** What’s in scope, what isn’t.
* **Highlight edge cases.** If something tricky exists, call it out.
* **Include testing expectations.** Especially unit tests — the more the better.
* **Avoid prescribing exact implementations.** Let the agent choose how.

---

## 🤖 For Agents

### You Are the Context

Agents are developers — and often the same ones writing the Plan. You have the most complete view of the codebase and its needs. Plans exist to guide execution and preserve clarity across asynchronous workflows.

Each task consists of:

* A GitHub Issue with a link to a DUCA Plan
* That Plan contains:

  * Links to relevant context (`*.Agent.md` files)
  * Rationale and boundaries for the work
  * Testing expectations

### Execution Rules

1. **Follow the Plan’s intent.** You own the implementation.
2. **Read all referenced `*.Agent.md` files** before editing any code.
3. **Update context files** if behaviors or conventions change.
4. **Do not create new Plans** unless explicitly instructed.
5. **Do not edit other Plans.** Each Plan is immutable.
6. **Do not assume memory.** Plans are stateless and self-contained.
7. **Write strong unit tests.** Integration tests only if the Plan says so.

> 🛠 General-purpose utilities (like logging, encryption, etc.) often have their own `X.Agent.md`. Check next to the file before using it.

---

## ✅ Success Looks Like

* The agent modifies only the scoped files.
* Code changes follow all referenced context rules.
* Context files (`*.Agent.md`) are updated where needed.
* Tests are added as instructed and follow naming rules.
* Code compiles, tests pass.
* PR is created with no extra scope or side work.

---

## 🧭 Philosophy

DUCA isn’t about managing human process. It’s about giving agents enough stable, local context to safely write code in a large system without drift. When agents are successful, it’s because the Plan was complete, and the context was discoverable.

If the PR doesn’t fit — fix the Plan or the context, not the agent.
