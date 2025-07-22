# Data Drafts

## Draft: Implement QuestDiscoveryETL
## Agent
This is bit different than usual. So here we have two ways to discover a quest. The QuestCategoryEndpoint and the QuestAreaEndpoint. We need to create a new ETL called QuestDiscoveryETL. This ETL will be responsible for discovering quests based on the category and area endpoints.They should loop through each of their DbContext's rows calling each endpoint and get a list of quest ids to potentially add. The ETL should first get all ids currently in the database. For each id that comes back, a quest stub should be created with the id, a status of NEEDS_ENRICHMENT, and a QuestIdentifier of either CATEGORY or AREA. The QuestIdentifierId should be set to the id of the category or area. The date should be set. The ETL should then save these stubs to the database. If a stub already exists, it should not be added again..



