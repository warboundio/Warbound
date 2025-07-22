# Data Drafts


## Draft: Implement JournalExpansionETL
## Agent
Now we need to enrich the data from JournalExpansionsIndexETL. https://us.api.blizzard.com/data/wow/journal-expansion/68?namespace=static-us&locale=en_US is the URL to use and an example output is JournalExpansion.json. This should set the ETLStatus to complete. The fields we want are a semicolon delimited list of DungeonIds: 'dungeons.id' for each in the array. Also RaidIds as it's own semicolon delimited list. let's use sensible defaults for maxlength on strings (2047 for most, 255 for names, etc.). It should grab all achievements in the database that 'NEED_ENRICHMENT'.

## Draft: Implement JournalInstanceMediaETL
## Agent
Finally for the dungeon journal we want to implement the JournalInstanceMediaETL. https://us.api.blizzard.com/data/wow/media/journal-instance/63?namespace=static-us&locale=en_US is the URL to use and an example output is JournalInstanceMedia.json. This should set the ETLStatus to complete. The fields we want are 'id' (the one supplied to the endpoint) and 'assets.value' (the first image URL). This will be used to display the dungeon journal images in the UI. Follow patterns like the ItemMediaEndpoint. Check the 'AchievementIndex.json' for an example output. let's use sensible defaults for maxlength on strings (2047 for most, 255 for names, etc.). This will use it's 'indexes' from all of the Journal Dungeons and Raids from teh JournalExpansion object.

## Draft: Implement AchievementETL
## Agent
We want to now enrich the data from AchievementsIndexETL. https://us.api.blizzard.com/data/wow/achievement/1?namespace=static-us&locale=en_US is the URL to use and an example output is Achievement.json. This should set the ETLStatus to complete. The fields we want are 'description', 'reward', 'points', 'icon', 'criteria.id' (a semicolon delimited list), and 'criteria.type' (a semicolon delimited list). Follow patterns like the ItemAppearanceETL. We want the 'description', 'category.name', 'reward_description', 'reward_item.id, 'reward_item.name'. Now keep in mind, reward_description reward_item.id and reward_item.name (and reward_item the object) may not exist. this is a *special case*. where you can default these to string empty if they do not exist. let's use sensible defaults for maxlength on strings (2047 for most, 255 for names, etc.). It should grab all achievements in the database that 'NEED_ENRICHMENT'._

## Draft: Implement AchievementMediaETL
## Agent
We want to implement the AchievementMediaETL. https://us.api.blizzard.com/data/wow/media/achievement/6?namespace=static-us&locale=en_US is the URL to use and an example output is AchievementMedia.json. This should set the ETLStatus to complete. The fields we want are 'id' (the one supplied to the endpoint) and URL 'assets.value' (the first image URL). This will be used to display the achievement images in the UI. Follow patterns like the ItemMediaEndpoint. Check the 'AchievementIndex.json' for an example output. let's use sensible defaults for maxlength on strings (2047 for most, 255 for names, etc.). It will get a list of ids from the achievements already in the database and ensure there's no duplicates.
