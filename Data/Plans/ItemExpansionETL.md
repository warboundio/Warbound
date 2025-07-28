# Plan: Item Expansion ETL

## Intent  
Create an ETL process to map items to their corresponding World of Warcraft expansions by analyzing journal encounter and expansion data, enabling expansion-based filtering and organization of collectible items.

## Context  
Currently, the system lacks expansion mapping for items, which prevents users from filtering collectibles by expansion. This builds on existing JournalExpansion and JournalEncounter data that contains the relationships needed to determine which expansion each item belongs to through the dungeon/raid → instance → encounter → item hierarchy.

## In Scope  
- Create ItemExpansion entity with item_expansion table mapping ItemId to ExpansionId
- Implement ItemExpansionETL process that analyzes JournalEncounter and JournalExpansion data
- Map items to expansions through encounter → instance → dungeon/raid → expansion relationships
- Set ExpansionId to -1 for items where expansion cannot be determined
- Add ItemExpansion collection to WarcraftData singleton
- Register new ETL job in the ETL system

## Acceptance Criteria  
- Given an item exists in the Items table, when ItemExpansionETL runs, then a corresponding ItemExpansion record is created
- Given a JournalEncounter contains item IDs, when ETL processes the encounter, then those items are mapped to the encounter's expansion via instance relationships
- Given an item cannot be mapped to any expansion, when ETL processes that item, then ExpansionId is set to -1
- Given ItemExpansionETL completes successfully, when querying ItemExpansion data, then all unique item IDs from the Items table have corresponding expansion mappings