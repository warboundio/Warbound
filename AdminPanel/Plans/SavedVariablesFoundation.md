# DUCA Plan: Initialize SavedVariables Foundation

This plan establishes the foundational SavedVariables table structure (`WarboundData`) for the World of Warcraft addon, creating a persistent data storage schema that will hold encoded collection data across all major collectible categories.

## 1. Read Addon Context

- Review **Addon/Addon.md** to understand the C# utility framework and LUA publishing system
- Examine **Addon/LUA/WarboundIO.toc** and **WarboundIO.lua** to understand current addon structure
- Study **Addon/LUA/Tools/Base90.lua** to understand the encoding utility already available
- Review **Addon/Drafts.md** to understand the five collection categories that will use this foundation

## 2. Understand SavedVariables Architecture

SavedVariables is a World of Warcraft addon mechanism that:
- Automatically persists data between game sessions
- Requires declaration in the addon's .toc file
- Creates global Lua tables that survive addon reloads
- Provides the foundation for all collection data storage in Warbound

## 3. Create WarboundData Table Structure

Implement **WarboundData** as the primary SavedVariables table with these properties:

### Core Schema
- **transmogs**: String field for Base90-encoded transmog appearance IDs
- **pets**: String field for Base90-encoded pet collection IDs  
- **mounts**: String field for Base90-encoded mount collection IDs
- **recipes**: String field for Base90-encoded recipe IDs across all professions
- **toys**: String field for Base90-encoded toy collection IDs
- **version**: String field for schema version tracking (start with "1.0")
- **lastUpdated**: String field for ISO timestamp of last data update

### Initial Values
- All collection fields initialize to empty string ("") to establish clear schema
- Version field initializes to "1.0"
- lastUpdated field initializes to current timestamp on first load

## 4. Update Addon Configuration

### Modify WarboundIO.toc
- Add `## SavedVariables: WarboundData` directive to register the persistent table
- Ensure this appears before any file includes in the .toc structure

### Enhance WarboundIO.lua
- Initialize WarboundData table with default schema if it doesn't exist

## 6. Future-Proofing Considerations

### Edge Case Handling
- Account for addon first-time installation (no existing SavedVariables)

## Implementation Details

### File Modifications Required
- **Addon/LUA/WarboundIO.toc**: Add SavedVariables directive
- **Addon/LUA/WarboundIO.lua**: Implement initialization logic and validation

### Scope Boundaries
- **In Scope**: 
  - Basic table structure and initialization
  - Schema version management foundation
  - Essential validation and recovery
  - Debug output for verification
- **Out of Scope**:
  - Actual collection data gathering (handled in future drafts)
  - Base90 encoding/decoding logic (already exists in Tools/Base90.lua)
  - Real-time update mechanisms (future draft)
  - UI components or user interaction

### Success Criteria
- WarboundData table persists correctly between game sessions
- All five collection categories have designated string fields
- Schema version tracking is functional and ready for future migrations
- Initialization logic handles edge cases gracefully
- Debug output confirms successful setup for development validation

### Dependencies
- Requires existing **Base90.lua** utility (already implemented)
- Builds on current **WarboundIO.lua** structure
- Uses standard World of Warcraft SavedVariables mechanism

## Why This Foundation Matters

This plan establishes the critical data persistence layer that all collection tracking depends on. Without this foundation:
- Individual collection drafts cannot store their encoded data
- Data would be lost between game sessions
- Future migration and versioning would be impossible
- The entire Warbound collection system would lack persistence

The schema design anticipates the five major collection categories defined in other drafts while remaining flexible for future additions. The version tracking enables safe evolution of the data structure as the addon grows in capability.

This foundation directly enables the subsequent implementation of transmog, pet, mount, recipe, and toy collection snapshots by providing the persistent storage mechanism they require.
