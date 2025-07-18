# ACEP Plan: Add '!clearandrun' Discord Command

This plan will add a new Discord bot command `!clearandrun` to the CommandCenterModule that combines lock clearing and manual ETL job execution in a single operation. This command provides developers with a powerful tool to immediately resolve stuck ETL jobs without requiring separate lock clearing and run commands.

## 1. Read Discord Module Instructions
- Open and follow **Core/Discords/ChannelHandler.Agent.md** to understand the Discord channel module patterns and command registration conventions.
- Review existing command implementations in **CommandCenterModule.cs** to understand the established patterns for command handling, error messaging, and Discord responses.

## 2. Read ETL System Instructions  
- Open and follow **Core/ETL/ETL.Agent.md** to understand the ETL job management system, distributed locking, and manual execution capabilities.
- Review **ETL/ETLs/_ETLs.Agent.md** to understand how ETL jobs are structured and executed within the system.

## 3. Understand Current Lock Management
Study the existing lock management patterns in **CommandCenterModule.cs**:
- Examine `HandleClearLocks()` method to understand how locks are cleared across all jobs
- Review `HandleRunCommand()` method to understand manual job execution patterns
- Understand the lock validation and error handling approaches used

## 4. Add Command Registration
Update **CommandCenterModule.cs** to register the new command:
- Add `!clearandrun` command registration in the `RegisterCustomCommands()` method
- Follow existing naming conventions and provide appropriate command description
- Ensure the command follows the established parameter pattern requiring an ETL job name

## 5. Implement Command Handler
Create new `HandleClearAndRunCommand()` method in **CommandCenterModule.cs** following these requirements:
- Accept ETL job name as required parameter with appropriate usage validation
- Clear lock only for the specified job (not all jobs like `!lockclear`)
- Immediately trigger manual execution after successful lock clearing
- Provide clear Discord feedback for each operation phase
- Handle edge cases: job not found, already running, execution failures
- Follow existing logging patterns for operational visibility

## 6. Lock Clearing Logic
Implement targeted lock clearing functionality:
- Query for the specific ETL job by name
- Clear only the `LockOwner` and `LockAcquiredAt` fields for that job
- Validate that the lock clearing was successful before proceeding to execution
- Handle database transaction appropriately

## 7. Manual Execution Integration
Integrate with existing manual execution infrastructure:
- Use `ETLRunner.RunJobManuallyAsync()` for consistent execution behavior
- Ensure proper error handling and status reporting
- Maintain consistency with existing `!run` command behavior and responses

## Implementation Details

### Command Syntax
- **Command**: `!clearandrun <job_name>`
- **Example**: `!clearandrun PetETL.RunAsync`
- **Response Pattern**: Multi-phase feedback showing lock clearing and execution status

### Error Handling Requirements
- **Missing job name**: Provide usage guidance similar to existing commands
- **Job not found**: Clear error message indicating the job doesn't exist
- **Lock clearing failure**: Specific error about the lock operation
- **Execution failure**: Separate error handling for the run operation
- **General exceptions**: Consistent with existing command error patterns

### Discord Response Flow
1. **Lock Clearing Phase**: `🔓 Clearing lock for {jobName}...`
2. **Lock Cleared**: `✅ Lock cleared for {jobName}`
3. **Execution Phase**: `🚀 Starting manual run for {jobName}...`
4. **Success**: `✅ {jobName} kicked off successfully`
5. **Error Responses**: Follow existing emoji and message patterns

### Database Operations
- Use the same `ETLContext` pattern as existing commands
- Clear locks only for the specified job, not system-wide
- Ensure atomic operations where possible
- Follow existing transaction patterns

### Logging Integration
- Use `Logging.Info()` for successful operations
- Use `Logging.Warn()` for operational warnings (job not found, etc.)
- Use `Logging.Error()` for exceptions with full exception details
- Follow existing log message formats for consistency

### Success Criteria
- New `!clearandrun` command is registered and discoverable
- Command accepts job name parameter with appropriate validation
- Lock clearing operates only on specified job
- Manual execution uses existing infrastructure
- Discord responses provide clear status updates
- Error handling covers all edge cases
- Logging follows established patterns
- Integration tests pass with existing command infrastructure

### Out of Scope
- Do not modify existing `!run` or `!lockclear` commands
- Do not change ETLRunner execution logic
- Do not modify ETL job database schema
- Do not create new ETL jobs or modify existing job definitions
- Do not implement additional command parameters or options
- Do not change Discord channel routing or message handling infrastructure