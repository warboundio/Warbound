# Data Drafts

## Draft: Implement QuestCategoryETL
## Agent
We need to now call the QuestCategory blizzard endpoint located at: https://us.api.blizzard.com/data/wow/quest/category/1?namespace=static-us&locale=en_US We need to call it for each index in the table. Since we're calling it, we know the id we're passing and that it's a QuestIdentifier of type category. Since we already have the quest object you'll just need to populate them and put them in the database with a status of NEEDS_ENRICHMENT. Please use QuestCategory.json to unit test the parsing.

## Draft: Implement QuestAreaETL
## Agent
We need to now call the QuestArea blizzard endpoint located at: https://us.api.blizzard.com/data/wow/quest/area/1?namespace=static-us&locale=en_US We need to call it for each index in the table. Since we're calling it, we know the id we're passing and that it's a QuestIdentifier of type area. Since we already have the quest object you'll just need to populate them and put them in the database with a status of NEEDS_ENRICHMENT. Please use QuestArea.json to test the parsing.

## Draft: Implement QuestTypeETL
## Agent
We need to now call the QuestType blizzard endpoint located at: https://us.api.blizzard.com/data/wow/quest/type/1?namespace=static-us&locale=en_US We need to call it for each index in the table. Since we're calling it, we know the id we're passing and that it's a QuestIdentifier of type 'type'. Since we already have the quest object you'll just need to populate them and put them in the database with a status of NEEDS_ENRICHMENT. Please use QuestType.json to test the parsing.

## Draft: Implement QuestETL
## Agent
Ok this will be the most complicated one, so while we want to watch others' patterns for coding standards this will be a bit more specific to itself. We have a Quest.json. We need to unit test that the endpoint we're going to create can parse it properly. The ValidatorETL should check the schema. We have three ETLs creating these Quest objects: QuestCategoryETL, QuestAreaETL, and QuestTypeETL. We need to call the blizzard endpoint located at: https://us.api.blizzard.com/data/wow/quest/2?namespace=static-us&locale=en_US for each object with a status of NEEDS_ENRICHMENT. We need to populate the Quest object with the data from the endpoint and then put it in the database with a status of COMPLETE. So it looks like i misunderstood the structure and schema a bit. On this object we have BOTH QuestType and QuestArea. So we need to REMOVE the option for 'QuestIdentifier' to be a TYPE. We need to ADD a property to the Quest object called QuestTypeId which will be parsed from our Quest.json at the property 'type.id' (if it exists otherwise 0). We need to check if it has an 'Area' property as the example json does, if it does, we need to set the QuestIdentifier to AREA and set the 'area.id'. I added a 'Quest2.json' to the repo to demonstrate how a QuestCategory type of 'QuestIdentifier' should look like. Here we need to set the QuestIdentifier as 'CATEGORY' and the QuestIdentifierId to 'category.id'. Ok now let's look at additional properties needed, We need a semicolon delimited list of RewardItems. Quest.json has an example structure where 'rewards.items.choice_of' exists and Quest2 does not. If it does exist, we want to get a semi column delimited list of the 'rewards.items.choice_of.item.id' for each of the choices. We do not want the requirements or playable_specializations. There's a lot of information we don't need in here. 

