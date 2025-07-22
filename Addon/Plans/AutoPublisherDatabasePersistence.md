# Plan: AutoPublisher Database Persistence

## Intent  
Add database persistence functionality to the AutoPublisher to save parsed WoW addon data with entity-specific logic for each data type.

## Context  
The AutoPublisher currently parses saved variables files and extracts game data but doesn't persist it to the database. This plan builds on the existing WarboundDataParser functionality to complete the data flow by implementing database operations with specific requirements for each entity type.

## In Scope  
- Add database persistence for PetBattleLocation entities (simple insert with GUID)
- Add database persistence for LootLogEntry entities (simple insert with GUID)  
- Add database persistence for NPCKillCount entities (aggregate counts for existing records or create new ones)
- Add database persistence for Vendor entities (delete any existing vendors by that key and insert the new one)
- Add database persistence for VendorItem entities (delete all existing items for that vendor id and then insert all the new vendor items)

## Acceptance Criteria  
- Given parsed PetBattleLocation data, when AutoPublisher processes it, then all records are inserted into the database
- Given parsed LootLogEntry data, when AutoPublisher processes it, then all records are inserted into the database
- Given parsed NPCKillCount data, when AutoPublisher processes it, then counts are added to existing records or new records are created
- Given parsed Vendor data, when AutoPublisher processes it, it deletes any existing vendor data tied to that key and inserts the new one
- Given parsed VendorItem data, when AutoPublisher processes it, then deletes all vendor items attached to that vendor and the inserts all the new ones.

## Developer Note
I've updated this a bit specifically the more complex ones. As we do want to track as new items come. But we also want to track when items leave. Or when prices change, etc. So I think clearing out anything tied to the Vendor or VendorItem for that key and then inserting what we saw is the easiest way to keep this up to date.
