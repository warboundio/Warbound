# Plan: Implement JournalExpansionsIndexETL

## Intent  
Add Journal Expansion index ETL to collect expansion ID and name data from Blizzard's API, creating stub records for future enrichment.

## Context  
Following the established pattern of index ETLs that seed the database with minimal records containing only core identifiers and names. This builds on the existing BlizzardAPI endpoint infrastructure and ETL framework to support the Journal Expansion data needed for dungeon journal organization. The expansion data serves as foundational reference information for organizing dungeon and raid content by game expansion.

## In Scope  
- Create JournalExpansion domain model with Id, Name, Status, and LastUpdatedUtc properties
- Implement JournalExpansionIndexEndpoint to parse tiers array from API response
- Add comprehensive unit tests for endpoint parsing
- Create JournalExpansionIndexETL following RunnableBlizzardETL pattern
- Update BlizzardAPIContext to include JournalExpansions DbSet
- Update WarcraftData singleton to include JournalExpansions dictionary

## Acceptance Criteria  
- Given the Blizzard API endpoint https://us.api.blizzard.com/data/wow/journal-expansion/index?namespace=static-us&locale=en_US, when JournalExpansionIndexEndpoint.Parse() is called, then it returns a list of JournalExpansion objects with populated Id and Name fields
- Given a JournalExpansion object created by the index endpoint, when examining its properties, then Status equals NEEDS_ENRICHED and LastUpdatedUtc is set to current UTC time
- Given the JournalExpansionIndexETL runs successfully, when querying the database, then new expansion records are inserted with only missing expansions added
- Given the unit tests are executed, when parsing the JournalExpansionsIndex.json fixture, then specific expansion records like "Classic" (id: 68) and "Dragonflight" (id: 503) are correctly parsed and validated