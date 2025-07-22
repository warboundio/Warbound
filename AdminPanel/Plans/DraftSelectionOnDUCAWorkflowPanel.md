# Plan: Draft Selection on DUCA Workflow Panel

## Intent  
Implement draft filtering logic in the DUCA Workflow Panel to prevent showing drafts that are already in progress as active workflows.

## Context  
Currently, when a user selects a target project in the DUCA Workflow Panel, all drafts for that project are displayed regardless of whether they're already active workflows. This creates confusion as users see drafts that are actually already in progress. The system needs to cross-reference drafts against active workflows and only show drafts that aren't currently being worked on. Active workflow names follow the format "ProjectName | WorkflowTitle" where the title part should match exactly with draft titles.

## In Scope  
- Modify draft display logic in DUCA Workflow Panel to filter out active workflows
- Inject GitHubIssueMonitor service to access active workflow data
- Implement title extraction logic to handle "ProjectName | Title" format matching
- Ensure case-insensitive comparison between draft titles and workflow titles

## Acceptance Criteria  
- Given a selected project with drafts and active workflows, when the DUCA panel loads drafts, then only drafts that are NOT already active workflows are displayed
- Given an active workflow named "Data | Implement JournalEncountersIndexETL", when checking against a draft titled "Implement JournalEncountersIndexETL", then the draft should be filtered out
- Given drafts that don't match any active workflow titles, when the panel loads, then those drafts are displayed normally