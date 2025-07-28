# Plan: Quest Reward Items ETL

## Intent  
Create a QuestRewardItemsETL to identify and track all item IDs used as quest rewards, ensuring these reward items are included in the database as stubs for later enrichment by ItemETL.

## Context  
The existing Quest entity captures reward item data in a semicolon-delimited format in the RewardItems property. Following the established RecipeIngredientsETL pattern, this ETL will parse quest reward data to discover item IDs that aren't currently in the database. These discovered items will be created as stubs with NEEDS_ENRICHED status, allowing the existing ItemETL to later enrich them with complete Blizzard API data.

## In Scope  
- Create QuestRewardItemsETL following RunnableBlizzardETL pattern to process complete quests
- Parse semicolon-delimited RewardItems strings to extract item IDs
- Create Item records with NEEDS_ENRICHED status for reward items not currently in database
- Integrate with existing ETL pipeline so ItemETL automatically processes discovered reward items
- Follow established ETL patterns for error handling and status management

## Acceptance Criteria  
- Given quests with Status COMPLETE containing RewardItems data, when QuestRewardItemsETL processes them, then missing reward item IDs are added to Items table with NEEDS_ENRICHED status
- Given RewardItems string "12345;67890;54321", when parsed by QuestRewardItemsETL, then item IDs 12345, 67890, and 54321 are created as Item records if they don't exist
- Given newly created reward Item records, when ItemETL runs, then reward items are enriched with complete Blizzard API data
- Given the QuestRewardItemsETL follows the same pattern as RecipeIngredientsETL, then it integrates seamlessly with existing ETL infrastructure