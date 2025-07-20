## 🔧 For Contributors

### Plans & Context

**Plans** live in `Project/Plans/` and describe work units. Focus on intent, edge cases, and deviations from norms. Context lives in `*.Agent.md` files next to source code.

**Writing Plans:**
* State why, not how
* Link relevant `*.Agent.md` files  
* Define scope and boundaries
* Highlight edge cases and testing needs

### Drafts

**Drafts** are work stubs for single PRs. Format:
```
## Draft: <Title>
### <Agent> or <Developer>
Brief scope description with clear boundaries.
```

---

## 🤖 For Agents

### Execution Rules

1. **Follow the Plan's intent.** You own implementation details.
2. **Read referenced `*.Agent.md` files** before editing code.
3. **Update context files** if behaviors change.
4. **Write strong unit tests** unless Plan specifies otherwise.

### Success Criteria

* Modify only scoped files
* Follow all referenced context rules
* Update `*.Agent.md` files when needed
* Code compiles, tests pass
* Remove completed drafts from `/Project/Drafts.md`
* Update `/Project/Project.md` to reflect current state

---

## 🧭 Philosophy

DUCA provides agents stable, local context to write code safely in large systems. When agents succeed, it's because the Plan was complete and context was discoverable.

If the PR doesn't fit — fix the Plan or context, not the agent.