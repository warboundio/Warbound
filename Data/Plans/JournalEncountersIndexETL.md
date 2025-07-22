# Plan: Implement JournalEncountersIndexETL

## Intent  
Implement ETL functionality to collect journal encounter index data from Blizzard's API, following existing patterns to stub out encounter data for future enrichment.

## Context  
This builds on the established ETL patterns used by ProfessionIndexETL and other index ETLs. The JournalEncountersIndex.json provides sample data structure from the Blizzard API endpoint. This initial implementation will stub out the data structure with only id and name populated, setting the stage for a future enrichment ETL to populate additional encounter details.

## In Scope  
- Create JournalEncounter entity class with appropriate table mapping and default values
- Implement JournalEncounterIndexEndpoint to parse API response from journal-encounter/index
- Implement JournalEncounterIndexETL following the RunnableBlizzardETL pattern
- Update BlizzardAPIContext to include JournalEncounters DbSet
- Update WarcraftData singleton to include JournalEncounters dictionary
- Remove corresponding draft from Drafts.md file

## Acceptance Criteria  
- Given the Blizzard API endpoint returns journal encounter index data, when the ETL runs, then JournalEncounter entities are created with id and name populated and status set to NEEDS_ENRICHED
- Given existing journal encounters in the database, when the ETL runs, then only new encounters are processed and saved
- Given the JournalEncounter entity, when viewed in the database, then it follows the wow schema table naming convention and has appropriate MaxLength constraints
- Given the ETL completes successfully, when checking the database, then all string fields except id and name contain empty strings and integer fields contain -1