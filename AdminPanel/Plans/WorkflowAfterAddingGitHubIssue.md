# Plan: Workflow After Adding a GitHub Issue

## Intent

After successfully creating a GitHub issue through the DUCA workflow panel, the system needs to provide user feedback, persist tracking data, and prepare for subsequent workflow management. Currently, the workflow creates the GitHub issue but provides no feedback to the user and loses the ability to track the issue's progress through the development pipeline.

## Context

The AdminPanel DUCA workflow creates GitHub issues but lacks:
- User feedback on successful creation
- Local persistence of issue metadata
- Connection between created issues and their development status
- Form state management after submission

This plan establishes the foundation for tracking GitHub issues through their entire lifecycle in the Warbound development workflow.

## Scope

### In Scope
1. **Database Schema Enhancement**
   - Convert ETLContext to CoreContext for broader application data
   - Create GitHubIssue entity with essential tracking fields
   - Establish database migration for the new schema

2. **Form UX Improvements** 
   - Clear form fields after successful submission
   - Display success message with issue details
   - Handle error states gracefully

3. **Issue Persistence**
   - Store GitHub issue metadata in local database
   - Track issue ID, creation time, and title
   - Include WaitingForYou status for workflow management

### Out of Scope
- Issue status synchronization with GitHub API
- Complex workflow state management beyond basic tracking
- User authentication or access control
- Performance optimization for large issue volumes

## Technical Approach

### Database Changes
- Rename ETLContext to CoreContext to reflect expanded scope
- Create GitHubIssue entity following ETLJob pattern:
  - `Id` (Guid, Primary Key)
  - `IssueId` (int, GitHub issue number)  
  - `CreatedAt` (DateTime, when inserted)
  - `Name` (string, max length 1023, issue title)
  - `WaitingForYou` (bool, default false)
- Update dependency injection and migration scripts

### Form Enhancement
- Add success/error state management to Duca.razor
- Implement form field clearing after successful submission
- Display confirmation with issue ID and link

### Data Flow
1. User submits form → GitHub issue created
2. On success → Store issue metadata in database
3. Clear form fields and show success message
4. On error → Display error without clearing form

## Edge Cases & Testing Needs

- Handle GitHub API failures gracefully
- Ensure database constraints prevent duplicate issue tracking
- Test form behavior with network timeouts
- Validate proper form clearing across all field types
- Confirm success message displays correct issue information

## Success Criteria

- Form clears all fields after successful GitHub issue creation
- Success message displays with issue ID and GitHub link
- GitHubIssue record created in database with initial fields populated
- Error states handled without data loss
- Database migration completes without affecting existing ETL functionality
- All existing ETL operations continue working with CoreContext

## Referenced Context

- `Core/ETL/ETL.Agent.md` - Current ETL context and patterns
- `AdminPanel/Components/Pages/Admin/Duca.razor` - Current form implementation
- `Core/GitHub/` - GitHub API integration patterns