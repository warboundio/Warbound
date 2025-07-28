# Plan: Journal Encounter Items ETL

## Intent  
Create a JournalEncounterItemsETL that parses semicolon-separated item IDs from JournalEncounter.Items field and creates stub Item entries in the database for items that don't exist yet.

## Context  
Following the same pattern as RecipeIngredientsETL, we need to extract item references from JournalEncounter objects and ensure these items exist as stubs in the database so the ItemsETL can later enrich them with complete data from the Blizzard API.

## In Scope  
- Create JournalEncounterItemsETL class extending RunnableBlizzardETL
- Parse semicolon-separated item IDs from JournalEncounter.Items field  
- Create stub Item entries with NEEDS_ENRICHED status for missing items
- Follow existing ETL patterns and coding conventions

## Acceptance Criteria  
- Given JournalEncounter objects with COMPLETE status containing semicolon-separated item IDs in Items field, when JournalEncounterItemsETL runs, then stub Item entries are created for any item IDs not already in the database
- Given stub items are created, when they are saved, then they have Status = NEEDS_ENRICHED and current LastUpdatedUtc timestamp
- Given the ETL follows existing patterns, when reviewed, then it uses the same structure as RecipeIngredientsETL with appropriate naming and functionality