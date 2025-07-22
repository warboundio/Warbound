# Plan: Implement QuestETL

## Intent  
Implement a QuestETL system to enrich quest objects from Blizzard API, parsing quest data including type, area/category identification, and reward items.

## Context  
The Data project needs to process Quest objects with NEEDS_ENRICHMENT status by calling the Blizzard quest endpoint (https://us.api.blizzard.com/data/wow/quest/{id}?namespace=static-us&locale=en_US) to populate quest details. Based on JSON analysis, quests can have either area or category identifiers, and reward items need to be parsed into a semicolon-delimited list. The existing Quest entity structure needs modification to support QuestTypeId and remove the TYPE identifier option.

## In Scope  
- Modify Quest entity to add QuestTypeId property and remove TYPE from QuestIdentifier enum
- Create QuestEndpoint to parse Blizzard API quest JSON data with proper type, area/category, and reward item handling
- Implement QuestETL following existing RunnableBlizzardETL pattern to enrich NEEDS_ENRICHMENT quests
- Add unit tests for QuestEndpoint parsing logic covering both area and category quest types
- Update SchemaValidationETL to include Quest schema validation
- Parse reward items from rewards.items.choice_of structure into semicolon-delimited string

## Acceptance Criteria  
- Given a Quest with NEEDS_ENRICHMENT status, when QuestETL runs, then quest is populated with data from Blizzard API and marked COMPLETE
- Given Quest.json with area property, when parsed by QuestEndpoint, then QuestIdentifier is set to AREA and QuestIdentifierId to area.id
- Given Quest2.json with category property, when parsed by QuestEndpoint, then QuestIdentifier is set to CATEGORY and QuestIdentifierId to category.id
- Given quest JSON with rewards.items.choice_of array, when parsed, then RewardItems contains semicolon-delimited list of item.id values
- Given quest JSON with type.id property, when parsed, then QuestTypeId is set to type.id value or 0 if missing
- Given QuestEndpoint unit tests, when executed, then all parsing scenarios pass including edge cases for missing properties