# Plan: Split LootLogEntry into Quantity and Location Tracking

## Intent  
Split the current LootLogEntry into two separate entities to reduce data duplication and improve performance by aggregating item quantities and preventing duplicate location entries.

## Context  
Currently, LootLogEntry logs each individual loot event with complete details (NpcId, ItemId, Quantity, ZoneId, X, Y, CreatedAt), resulting in numerous duplicate entries for the same NPC/Item combinations and locations. This follows the same aggregation pattern already established by NPCKillCount, which uses a simple key-based approach with count accumulation.

## In Scope  
- Create LootItemSummary entity with composite key [NpcId, ItemId] to track total quantities
- Create LootLocationEntry entity with composite key [NpcId, X, Y, ZoneId] to track unique locations
- Remove the existing LootLogEntry entity and table
- Update Lua addon code to work with the new data structure
- Update any existing data parsing or processing logic

## Acceptance Criteria  
- Given an NPC drops the same item multiple times, when loot events are processed, then LootItemSummary quantity is incremented rather than creating duplicate records
- Given an NPC is killed at the same coordinates multiple times, when loot events are processed, then only one LootLocationEntry exists for those coordinates
- Given the existing LootLogEntry table exists, when migration occurs, then data is properly migrated to the new entities without loss
- Given Lua addon collects loot data, when data is sent to the backend, then it works with the new split entity structure