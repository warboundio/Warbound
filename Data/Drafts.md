# Data Drafts

## Draft: Implement QuestAreaIndexETL
## Agent
Following other patterns such as the RealmIndexETL, we want to implement the QuestAreaIndexETL. This will use the QuestAreaIndex.json as the output. The URL to use is https://us.api.blizzard.com/data/wow/quest/area/index?namespace=static-us&locale=en_US. This will complete this object, so set the status to complete. We only want the id and the name here. Let's also have the UTC time like the other objects and the status = complete. 

## Draft: Implement QuestCategoriesETL
## Agent
Following other patterns such as the RealmIndexETL, we want to implement the QuestCategoriesETL. This will use the QuestCategories.json as the output. The URL to use is https://us.api.blizzard.com/data/wow/quest/category/index?namespace=static-us&locale=en_US. This will complete this object, so set the status to complete. We only want the id and the name here. Let's also have the UTC time like the other objects and the status = complete.

## Draft: Implement QuestTypeIndexETL
## Agent
Following other patterns such as the RealmIndexETL, we want to implement the QuestTypeIndexETL. This will use the QuestTypeIndex.json as the output. The URL to use is https://us.api.blizzard.com/data/wow/quest/type/index?namespace=static-us&locale=en_US. This will complete this object, so set the status to complete. We only want the id and the name here. Let's also have the UTC time like the other objects and the status = complete.

## Draft: Add Quest Object
## Agent
So this won't exactly follow the plan like the other endpoints and ETLs have. Here we have three different ways to source out a 'quest'. The QuestArea, the QuestCategories, and QuestType. When we call something like QuestAreaEndpoint we're going to see all quests for that particular grouping. The same goes for the QuestCategories and QuestType. So we created a 'QuestIdentifier' enum to identify which of these the quest came from. QuestAreaIndex, QuestCategoriesIndex, and QuestTypeIndex all follow the same pattern, so let's just talk to QuestAreaIndex and follow it for all three. QuestAreaIndexETL will put an id and the name for each of the areas in the game. So our quest object needs to know the 'QuestIdentifier' (in this case the QuestArea) and the id that was assigned to it from the QuestAreaIndexETL. So our Quest object will have the following fields: Id, Name (511 max length), QuestIdentifier (QuestIdentifier enum), QuestIdentifierId (the id from the QuestAreaIndexETL, QuestCategoriesIndexETL, or QuestTypeIndexETL). The status will be set to NEEDS_ENRICHMENT_ and the UTC time will be set to the current time. Once we have that object complete and ready, we will populate it via the varying QuestIdentifiers. 
