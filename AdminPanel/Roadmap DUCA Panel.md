# DUCA Admin Workflow Panel

## Summary

This project introduces a new Blazor-based admin panel workflow system that wraps around GitHub. The goal is to streamline how DUCA agents are kicked off and how we track the lifecycle of features — from idea to merged PR — with as little busy work as possible.

We will introduce:
- UI-driven workflows (e.g., “Feature Implementation”, “Refactor”, etc.)
- GitHub issue and PR automation with templated context
- A real-time thread tracker for each task
- "Advance" buttons to progress work through defined stages
- Per-workflow templating and execution logic

This effort will be split into small, sequential implementation plans. Each “brick” will be sized for a single PR and checkpointed for validation.

---

## Key Concepts

- **Workflow Templates** – Define what happens at each stage. Example: `Feature Implementation` begins with Plan creation, then generates an Implementation task, then links to a PR.
- **Thread View** – Every initiated workflow shows its lifecycle: issue, plan, PR, review status, etc.
- **Advance Control** – After each step is done, the user hits "Advance" to continue the flow (e.g., merge PR, assign next issue).
- **GitHub Wrapper Service** – Handles issue/PR creation, commenting, labels, status polling, project board updates.

---

## Bricks of Work

Each of the following will be its own DUCA Plan and implemented via a single PR.

---

### 🔲 Brick 1: Project Scaffolding
**Goal:** Introduce new Blazor admin section for `Workflow Management`.

- Create a basic `/Admin/WorkflowPanel.razor` UI
- Add left-hand nav for access
- Show placeholder “Start New Workflow” and “Thread View” sections

---

### 🔲 Brick 2: Workflow Templates & Schema
**Goal:** Define reusable JSON/YAML schema to power workflows.

- A workflow is a list of steps with:
  - Step type (plan, issue, pr)
  - Required user input
  - Associated boilerplate templates
- Add sample: `FeatureImplementation.workflow.json`

---

### 🔲 Brick 3: GitHub API Wrapper Service
**Goal:** Introduce a thin GitHub API abstraction for internal use.

- Encapsulate:
  - CreateIssue, CreatePullRequest
  - CommentOnIssue
  - MoveCardToColumn
- Use existing GitHub token in `ApplicationSettings` (minimal scope for now)

---

### 🔲 Brick 4: Start Workflow UI
**Goal:** Let user initiate a new workflow via form-driven UX.

- Dropdown: Project selector
- Dropdown: Workflow type (loads template)
- Text inputs: Roadmap text, Developer notes, Optional extras
- On submit:
  - Uses GitHub wrapper to create issue
  - Adds labels/project metadata
  - Stores local workflow instance for tracking

---

### 🔲 Brick 5: Thread View + Polling
**Goal:** Display live workflow state.

- Given a workflow, show:
  - Created issue link
  - Any PRs attached (via auto-detected issue links)
  - Review status (mergeable, CI passing, etc.)
- Poll every 30s

---

### 🔲 Brick 6: Advance Button & Transitions
**Goal:** Allow user to trigger the next step in the workflow.

- After PR merge, show “Advance”
- Advance triggers the next workflow step:
  - Implementation issue → Plan
  - Plan → PR
  - Final step → “Complete”

---

## Future Extensions

- GitHub App tokens with tighter scope and webhooks
- Discord notifications
- Copilot response validation
- Inline Plan/PR review from the panel

---

## Notes

- All GitHub interactions are handled by a token in `ApplicationSettings` (or env).
- DUCA Agents remain unchanged — this panel just gives them a faster runway.
- Workflow tracking is eventually intended to replace Trello.

---

## Owner

This workflow panel is owned by the DUCA core team. New bricks will be written as Plans, reviewed, and executed by DUCA agents via standard processes.

