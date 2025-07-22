# Plan: QuestDiscoveryETL

## Intent  
Create a new ETL that discovers quest IDs from both QuestCategory and QuestArea endpoints, creating quest stubs for future enrichment.

## Context  
The system currently has QuestCategory and QuestArea entities populated via their respective index ETLs. We need to discover the actual quests associated with these categories and areas by calling the QuestCategoryEndpoint and QuestAreaEndpoint, which return lists of quest IDs that can be used to create quest stub records for future enrichment.

## In Scope  
- Create QuestDiscoveryETL class inheriting from RunnableBlizzardETL
- Loop through all QuestCategory records and call QuestCategoryEndpoint for each
- Loop through all QuestArea records and call QuestAreaEndpoint for each  
- Create Quest stub records with status NEEDS_ENRICHED for new quest IDs
- Set appropriate QuestIdentifier (CATEGORY or AREA) and QuestIdentifierId
- Avoid creating duplicate Quest records

## Acceptance Criteria  
- Given existing QuestCategory records, when QuestDiscoveryETL runs, then quest stubs are created with QuestIdentifier=CATEGORY and QuestIdentifierId set to the category ID
- Given existing QuestArea records, when QuestDiscoveryETL runs, then quest stubs are created with QuestIdentifier=AREA and QuestIdentifierId set to the area ID  
- Given quest IDs that already exist in the database, when QuestDiscoveryETL runs, then no duplicate Quest records are created
- Given newly discovered quest IDs, when QuestDiscoveryETL runs, then Quest records are created with Status=NEEDS_ENRICHED and LastUpdatedUtc set to current time