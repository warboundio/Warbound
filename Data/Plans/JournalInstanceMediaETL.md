# Plan: JournalInstanceMediaETL Implementation

## Intent  
Implement JournalInstanceMediaETL to collect and process journal instance media data from Blizzard's API, specifically extracting dungeon and raid images for display in the UI.

## Context  
The dungeon journal system requires visual media to enhance user experience. Following the established ETL patterns like ItemMediaETL, this builds on the existing JournalExpansion infrastructure to collect media assets for journal instances (dungeons and raids) from Blizzard's media API endpoints.

## In Scope  
- Create JournalInstanceMedia data model with Id and URL fields
- Implement JournalInstanceMediaEndpoint to fetch data from Blizzard API
- Build JournalInstanceMediaETL processing logic following RunnableBlizzardETL pattern
- Integrate with BlizzardAPIContext and WarcraftData singleton
- Source processing indexes from JournalExpansion DungeonIds and RaidIds

## Acceptance Criteria  
- Given a journal instance ID from JournalExpansion DungeonIds/RaidIds, when JournalInstanceMediaETL runs, then it fetches media data from `https://us.api.blizzard.com/data/wow/media/journal-instance/{id}?namespace=static-us&locale=en_US`
- Given successful API response, when parsing the JSON, then extract the "id" field and first "assets.value" URL and save to database
- Given successful processing, when ETL completes, then set ETLStatus to COMPLETE for processed records
- Given the new model, when integrated with context, then JournalInstanceMedia is available through BlizzardAPIContext and WarcraftData