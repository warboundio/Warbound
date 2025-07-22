# Plan: AutoPublisher Database Persistence

## Intent  
Add database persistence functionality to the AutoPublisher to save parsed WoW addon data with entity-specific logic for each data type.

## Context  
The AutoPublisher currently parses saved variables files and extracts game data but doesn't persist it to the database. This plan builds on the existing WarboundDataParser functionality to complete the data flow by implementing database operations with specific requirements for each entity type.

## In Scope  
- Add database persistence for PetBattleLocation entities (simple insert with GUID)
- Add database persistence for LootLogEntry entities (simple insert with GUID)  
- Add database persistence for NPCKillCount entities (aggregate counts for existing records or create new ones)
- Add database persistence for Vendor entities (insert only if not already exists)
- Add database persistence for VendorItem entities (insert only if composite key doesn't exist)

## Acceptance Criteria  
- Given parsed PetBattleLocation data, when AutoPublisher processes it, then all records are inserted into the database
- Given parsed LootLogEntry data, when AutoPublisher processes it, then all records are inserted into the database
- Given parsed NPCKillCount data, when AutoPublisher processes it, then counts are added to existing records or new records are created
- Given parsed Vendor data, when AutoPublisher processes it, then only new vendors are inserted into the database
- Given parsed VendorItem data, when AutoPublisher processes it, then only new vendor-item combinations are inserted into the database