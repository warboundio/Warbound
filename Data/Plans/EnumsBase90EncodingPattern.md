# Plan: Enums Base90 Encoding Pattern

## Intent  
Update Data project enums to follow a consistent UNKNOWN/UNKNOWN_ENCODING pattern that supports base90 encoding requirements where UNKNOWN = -1 and UNKNOWN_ENCODING uses values 89 or 8099 based on enum size.

## Context  
The base90 encoding system requires enum values to be in the range 0-X without negative values. Currently some enums only have UNKNOWN = -1 which cannot be encoded in base90. The established pattern adds UNKNOWN_ENCODING with a value of 89 (if max enum value < 70) or 8099 (if max enum value >= 90) to provide encoding compatibility while preserving the existing UNKNOWN = -1 for standard usage.

## In Scope  
- Add missing UNKNOWN = -1 to enums that don't have it (ETLStateType, SlotURLTypes, PriorityLevel)
- Add missing UNKNOWN_ENCODING values to enums following the pattern:
  - SlotType: add UNKNOWN_ENCODING = 89 (max value 19)
  - ETLStateType: add UNKNOWN = -1 and UNKNOWN_ENCODING = 8099 (max value 100)  
  - SlotURLTypes: add UNKNOWN = -1 and UNKNOWN_ENCODING = 89 (max value 23)
  - PriorityLevel: add UNKNOWN = -1 and UNKNOWN_ENCODING = 89 (max value ~1)
- Verify all existing tests continue to pass after updates

## Acceptance Criteria  
- Given an enum with max value < 70, when UNKNOWN_ENCODING is added, then it uses value 89
- Given an enum with max value >= 90, when UNKNOWN_ENCODING is added, then it uses value 8099  
- Given any enum in the Data project, when inspected, then it has both UNKNOWN = -1 and UNKNOWN_ENCODING with appropriate value
- Given all enum updates are complete, when tests are run, then all existing tests pass