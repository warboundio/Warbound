# Warbound Addon Drafts

## Draft: Split LootLogEntry
## Agent
Right now we are logging what was looted and at what X, Y, map location for each NPC. But I feel like we should split this into two different logs. One for key [NPCID and ItemId] where those are the primary key and we can simply increase the Quantity, much like we do with NPCKillCount. And then second would be focused just on the X, Y, and ZoneId. This would also have a unique key of [NPC, X, Y, ZoneId] so that we don't have a ton of duplicate entries. This will help us keep the data clean and organized.
