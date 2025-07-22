# Data Drafts


## Draft: Implement JournalInstanceMediaETL
## Agent
Finally for the dungeon journal we want to implement the JournalInstanceMediaETL. https://us.api.blizzard.com/data/wow/media/journal-instance/63?namespace=static-us&locale=en_US is the URL to use and an example output is JournalInstanceMedia.json. This should set the ETLStatus to complete. The fields we want are 'id' (the one supplied to the endpoint) and 'assets.value' (the first image URL). This will be used to display the dungeon journal images in the UI. Follow patterns like the ItemMediaEndpoint. Check the 'AchievementIndex.json' for an example output. let's use sensible defaults for maxlength on strings (2047 for most, 255 for names, etc.). This will use it's 'indexes' from all of the Journal Dungeons and Raids from teh JournalExpansion object.

## Draft: Implement QuestAreaIndexETL
## Agent
Following other patterns such as the RealmIndexETL, we want to implement the QuestAreaIndexETL. This will use the QuestAreaIndex.json as the output. The URL to use is https://us.api.blizzard.com/data/wow/quest/area/index?namespace=static-us&locale=en_US. This will complete this object, so set the status to complete. We only want the id and the name here. Let's also have the UTC time like the other objects and the status = complete. 

## Draft: Implement QuestCategoriesETL
## Agent
Following other patterns such as the RealmIndexETL, we want to implement the QuestCategoriesETL. This will use the QuestCategories.json as the output. The URL to use is https://us.api.blizzard.com/data/wow/quest/category/index?namespace=static-us&locale=en_US. This will complete this object, so set the status to complete. We only want the id and the name here. Let's also have the UTC time like the other objects and the status = complete.

## Draft: Implement QuestTypeIndexETL
## Agent
Following other patterns such as the RealmIndexETL, we want to implement the QuestTypeIndexETL. This will use the QuestTypeIndex.json as the output. The URL to use is https://us.api.blizzard.com/data/wow/quest/type/index?namespace=static-us&locale=en_US. This will complete this object, so set the status to complete. We only want the id and the name here. Let's also have the UTC time like the other objects and the status = complete.

## Draft: Implement QuestAreaETL
## Agent
NYI