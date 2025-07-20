# GitHubIssueService

> Part of the DUCA system. Automates issue creation, labeling, assignment, and triage for GitHub Copilot agents.

---

## 🧠 Purpose

`GitHubIssueService` exists to **minimize friction** when assigning new work to Copilot agents.

In DUCA, we strive to **lower the cost of administrative overhead** when managing autonomous agents. Every new task should be quick to enter, easy to track, and immediately actionable.

This service ensures that whenever we submit new work:
- It gets created as a well-formed GitHub issue.
- It’s placed on the correct **Project V2 board** (e.g., `WarboundIO` agents).
- It's assigned the appropriate **Status column** (e.g., `Agents in Progress`).
- Copilot is **automatically assigned** and can begin execution right away.
- Labels (e.g., `agent-work`) are applied to route/filter issues downstream.

This tight integration allows us to submit goals from higher-level admin tooling with minimal manual effort.

---

## ⚙️ Usage

```csharp
int issueNumber = await GitHubIssueService.Create(
    title: "Refactor InventoryProcessor to use caching",
    body: "See DUCA plan `inventory-caching.md` for context. This will improve perf by avoiding redundant DB lookups.",
    assignToCopilot: true,
    labels: new[] { "agent-work", "perf" }
);
