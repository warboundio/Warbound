# DUCA Plan: Add '!clearandrun' Discord Command

Add combined command to clear specific ETL job lock and execute immediately.

**Context:** Follow **Core/Discords/ChannelHandler.Agent.md** and **Core/ETL/ETL.Agent.md**

## Implementation

**CommandCenterModule.cs:**
- Register `!clearandrun` command in `RegisterCustomCommands()`
- Create `HandleClearAndRunCommand()` method

## Key Behavior

**Lock Clearing:** Clear lock for specified job only (not all jobs like `!lockclear`)
**Execution:** Trigger manual run immediately after lock clear
**Feedback:** Provide Discord response for each operation phase

## Error Handling

- Validate ETL job name parameter
- Handle lock clearing failures gracefully  
- Report execution failures with appropriate messaging
- Follow existing command error patterns

## Testing

Add `ItShouldHandleClearAndRunCommand` test covering successful flow and error cases.