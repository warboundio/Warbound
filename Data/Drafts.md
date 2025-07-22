# Data Drafts


## Draft: Implement JournalInstanceMediaETL
## Agent
Finally for the dungeon journal we want to implement the JournalInstanceMediaETL. https://us.api.blizzard.com/data/wow/media/journal-instance/63?namespace=static-us&locale=en_US is the URL to use and an example output is JournalInstanceMedia.json. This should set the ETLStatus to complete. The fields we want are 'id' (the one supplied to the endpoint) and 'assets.value' (the first image URL). This will be used to display the dungeon journal images in the UI. Follow patterns like the ItemMediaEndpoint. Check the 'AchievementIndex.json' for an example output. let's use sensible defaults for maxlength on strings (2047 for most, 255 for names, etc.). This will use it's 'indexes' from all of the Journal Dungeons and Raids from teh JournalExpansion object.




