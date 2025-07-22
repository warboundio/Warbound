# Data Drafts



## Draft: Implement JournalEncounterETL
### Agents
Now that we have the data stubbed out for 'JournalEncountersIndexETL' we can now enrich the data. https://us.api.blizzard.com/data/wow/journal-encounter/89?namespace=static-us&locale=en_US is your URL. Following patterns like the MountEndpoint and ETL we want to enrich the data. Property 1 to add: Items. A semi colon delimited list that will look at 'items' and the array inside of it and grab the items.item.id (not the items.id). max length of 2047. We are omitting hte sections. We do want the 'instance.name'. We want the 'instance.id'. We want the 'category.type'. and a semicolon delimited list of 'modes.type'. let's use sensible defaults for maxlength on strings (2047 for most, 255 for names, etc.).



## Draft: Implement JournalExpansionETL
### Agents
Now we need to enrich the data from JournalExpansionsIndexETL. https://us.api.blizzard.com/data/wow/journal-expansion/68?namespace=static-us&locale=en_US is the URL to use and an example output is JournalExpansion.json. This should set the ETLStatus to complete. The fields we want are a semicolon delimited list of DungeonIds: 'dungeons.id' for each in the array. let's use sensible defaults for maxlength on strings (2047 for most, 255 for names, etc.).

## Draft: Implement JournalInstanceMediaETL
### Agents
Finally for the dungeon journal we want to implement the JournalInstanceMediaETL. https://us.api.blizzard.com/data/wow/media/journal-instance/63?namespace=static-us&locale=en_US is the URL to use and an example output is JournalInstanceMedia.json. This should set the ETLStatus to complete. The fields we want are 'id' (the one supplied to the endpoint) and 'assets.value' (the first image URL). This will be used to display the dungeon journal images in the UI. Follow patterns like the ItemMediaEndpoint. Check the 'AchievementIndex.json' for an example output. let's use sensible defaults for maxlength on strings (2047 for most, 255 for names, etc.).

## Draft: Implement AchievementsIndexETL
### Agents
We want to get a stub for each achievement in the game. https://us.api.blizzard.com/data/wow/achievement/6?namespace=static-us&locale=en_US Check the 'AchievementIndex.json' for an example output. Status should be set to NEEDS_ENRICHED and LastUpdatedUTC should be set to DateTime.UtcNow. The id and name should be populated. Follow patterns like the ItemAppearanceIndexETL. Fail fast, assume the properties will be parsed correctly, and ensure all properties are set to their defaults (empty strings, -1 for ints, etc.). We want to stub out the data for it to be enriched later. let's use sensible defaults for maxlength on strings (2047 for most, 255 for names, etc.).

## Draft: Implement AchievementETL
### Agents
We want to now enrich the data from AchievementsIndexETL. https://us.api.blizzard.com/data/wow/achievement/1?namespace=static-us&locale=en_US is the URL to use and an example output is Achievement.json. This should set the ETLStatus to complete. The fields we want are 'description', 'reward', 'points', 'icon', 'criteria.id' (a semicolon delimited list), and 'criteria.type' (a semicolon delimited list). Follow patterns like the ItemAppearanceETL. We want the 'description', 'category.name', 'reward_description', 'reward_item.id, 'reward_item.name'. Now keep in mind, reward_description reward_item.id and reward_item.name (and reward_item the object) may not exist. this is a *special case*. where you can default these to string empty if they do not exist. let's use sensible defaults for maxlength on strings (2047 for most, 255 for names, etc.).

## Draft: Implement AchievementMediaETL
### Agents
We want to implement the AchievementMediaETL. https://us.api.blizzard.com/data/wow/media/achievement/6?namespace=static-us&locale=en_US is the URL to use and an example output is AchievementMedia.json. This should set the ETLStatus to complete. The fields we want are 'id' (the one supplied to the endpoint) and 'assets.value' (the first image URL). This will be used to display the achievement images in the UI. Follow patterns like the ItemMediaEndpoint. Check the 'AchievementIndex.json' for an example output. let's use sensible defaults for maxlength on strings (2047 for most, 255 for names, etc.).
