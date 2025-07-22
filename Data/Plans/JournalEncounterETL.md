# Plan: Implement JournalEncounterETL

## Intent  
Create a JournalEncounterETL system to enrich journal encounter data by calling the Blizzard API for individual encounters and populating additional fields like items, instance information, category, and game modes.

## Context  
The JournalEncounterIndexETL already creates stub records with NEEDS_ENRICHED status. Following the established patterns of MountEndpoint and MountETL, we need to create an endpoint and ETL process to enrich these stubs with detailed data from the Blizzard API journal encounter endpoint.

## In Scope  
- Create JournalEncounterEndpoint class to call individual encounter API endpoints
- Create JournalEncounterETL class to process encounters with NEEDS_ENRICHED status
- Parse and populate Items field with semicolon-delimited list from items.item.id
- Parse and populate InstanceName and InstanceId from instance data
- Parse and populate CategoryType from category.type
- Parse and populate ModesTypes with semicolon-delimited list from modes.type

## Acceptance Criteria  
- Given a JournalEncounter with NEEDS_ENRICHED status, when JournalEncounterETL runs, then the encounter is enriched with API data and status set to COMPLETE
- Given the journal encounter API response, when parsing items array, then Items field contains semicolon-delimited list of items.item.id values
- Given the journal encounter API response, when parsing instance data, then InstanceName and InstanceId are populated correctly
- Given the journal encounter API response, when parsing category and modes, then CategoryType and ModesTypes are populated with appropriate values