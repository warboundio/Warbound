## 🔧 For Contributors

### Plans & Context

**Plans** live in `Project/Plans/` and describe work units. Focus on implementation details that need developer approval. Context lives in `*.Agent.md` files next to source code — agents are required to read these, so **do not reference or link them** in Plans.

**Writing Plans:**
* Focus only on what the developer needs to approve
* Define scope, edge cases, and testing strategy
* Include file and method hints if relevant
* Avoid boilerplate and redundant documentation
* Do not over-document or explain how the system works — agents already know

### Drafts

**Drafts** are scoped stubs for a single PR. Format:
```
## Draft: <Title>
### <Agent> or <Developer>
Brief scope description with clear boundaries.
```
Draft text always get removed from `/Project/Drafts.md` when implemented, regardless of workflow.

---

## 🤖 For Agents

### Workflows

**Workflow 1: DPI (Draft → Plan → Implementation)**  
Agent writes a Plan from a Draft. Developer reviews it in a PR. Once approved (via "Request Changes"), the same agent implements it.  
✅ Remove the Draft from `Drafts.md`  
✅ Keep the Plan in `Plans/`  
✅ Update `Project.md` to reflect the new feature
✅ Actual implementation can not begin until the developer requests changes after reviewing the initial PR that is just the plan.

**Workflow 2: One Shot (Direct Implementation)**  
GitHub issue body *is* the instruction.  
✅ Implement directly  
✅ Update `Project.md` if appropriate  
✅ Remove any matching Drafts


**Workflow 3: PI (Plan → Implementation)**  
The GitHub issue *is* the draft. Agent creates a Plan in PR, gets it approved, and then implements it.  
✅ Keep Plan in `Plans/`  
✅ Update `Project.md`
✅ Actual implementation can not begin until the developer requests changes after reviewing the initial PR that is just the plan.

---

### Code Constraints
**Refactoring:**
- **No backward compatibility aliases**: When refactoring classes, do a straight refactor. Do not create alias classes for backward compatibility.
- **Complete reference updates**: Update all references to the old class name throughout the codebase.
- **Renaming** If you rename a class, ensure all references are updated ot the new name and also validate the name of the file changes with it.

**Database:**
- **No automatic Entity Framework migrations**: Do not create migration files. Developers handle database changes manually.
- **Entity definition only**: Define entities in code but let developers manage the actual database schema changes.

### Execution Rules

1. Follow the Plan exactly. Do not guess or go beyond the approved scope.
2. Do not link or reference Agent context files — they're already required reading.
3. Do not update Plans after approval. They are immutable once implementation begins.
4. Update `*.Agent.md` files **only when purpose, vision, or intent changes**.
5. Write strong unit tests for C#. Unless explicitly told not to. LUA and Blazor Display code is not required to test.

---

## 🧭 Philosophy

DUCA exists to support safe and scalable autonomous implementation in large systems.  
Agents already know how to code.  
Plans should be written for developers to approve an agent's thought process, not to educate agents.
