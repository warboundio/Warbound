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

### Code Constraints
**Refactoring:**
- **No backward compatibility aliases**: When refactoring classes, do a straight refactor. Do not create alias classes for backward compatibility.
- **Complete reference updates**: Update all references to the old class name throughout the codebase.
- **Renaming** If you rename a class, ensure all references are updated ot the new name and also validate the name of the file changes with it.

**Database:**
- **No automatic Entity Framework migrations**: Do not create migration files. Developers handle database changes manually.
- **Entity definition only**: Define entities in code but let developers manage the actual database schema changes.

### Execution Rules

1. **Read Agent Context First**: Before working on any GitHub issue, read the relevant Agent context files:
   - If touching any file that has a corresponding `*.Agent.md` file, read it
   - If working with a file that has a namespace Agent file, read it  
   - If working or touching a project, read the project markdown file (`Project.md`)
   - These provide essential context for making the right decisions

2. Follow the Plan exactly. Do not guess or go beyond the approved scope.

3. Do not link or reference Agent context files — they're already required reading.

4. Do not update Plans after approval. They are immutable once implementation begins.

5. Update `*.Agent.md` files **only when purpose, vision, or intent changes**.

6. Write strong unit tests for C#. Unless explicitly told not to. LUA and Blazor Display code is not required to test.

7. **Fail-Fast Philosophy**:
   - Code should be fail-fast - assume the happy path and throw exceptions when things go wrong
   - Avoid excessive try/catch blocks and defensive programming
   - Only the public API entry points should have try/catch for graceful error handling
   - Internal methods should throw exceptions rather than returning false/null for errors
   - Use the Core.Logs.Logging system, never Console.WriteLine
   - Avoid XML comments unless really important
   - Code should be self-documenting

8. **Class Design**:
   - Avoid static classes - classes should be instantiated instead of being static
   - Static ways to access functionality is fine, but avoid classes with all static methods
   - Prefer instance-based design for better testability and flexibility

9. **Testing Guidelines**:
   - Write tests aimed at functions. We want to avoid integration tests. The smaller the unit test, the better.
   - Do not test trivial assertions like it returns true or false. Each test should be meaningful or it should not exist.
   - Tests can create/modify files in `C:\Applications\Warbound\temp` for validation
   - Do not modify application settings, environment variables, or actual system files
   - Clean up any test artifacts after test completion
   - Only one test should perform file system operations due to parallel execution
   - Tests should verify real functionality, not just return value types

10. **Boy Scout Rule**: If your pull request is including edits to a file, that edit is eligible to be 'cleaned up'. 
	- The Boy Scout Rule states that you should leave the code cleaner than you found it.
	- So if anything no longer fits our coding standards, please correct it.
	- This does not include refactoring, adding new logic, or changing functionality.
	- Things like renaming variables, removing unused variables, and correcting formatting issues are all acceptable.
	- If the context of a method has changed, that method's name should be updated to reflect the new context.

### Agent Context Files

**Project Files (`Project.md`)**: Focus on the vision, intention, and philosophy of the project. Describe the "why" and developer thoughts, not implementation details or folder structure accounting.

**Class/Namespace Files (`*.Agent.md`)**: Provide context about specific components, focusing on purpose and design intentions rather than usage documentation.

All Agent context files emphasize intention and vision over implementation details.

---

## 🧭 Philosophy

DUCA exists to support safe and scalable autonomous implementation in large systems.  
Agents already know how to code.  
Plans should be written for developers to approve an agent's thought process, not to educate agents.
