# Warbound Addon Drafts

## Draft: Initialize SavedVariables Format
### Agent
Create the foundational SavedVariables table structure (`WarboundData`) to hold encoded data strings for each collection category. Include an empty string to define a clear schema layout.

## Draft: Implement Transmog Collection Snapshot
### Agent
Track unlocked transmog appearances. Encode appearance IDs via Base90 and store the result in `WarboundData.transmogs`. Includes relevant event hooks.

## Draft: Implement Pet Collection Snapshot
### Agent
Track unlocked battle pets and companions. Encode them via Base90 and store the string in `WarboundData.pets`. Includes relevant event hooks.

## Draft: Implement Mount Collection Snapshot
### Agent
Track collected mounts. Hook appropriate events and encode them into `WarboundData.mounts`.

## Draft: Implement Recipe Collection Snapshot
### Agent
Track known recipes across all professions. Listen for profession updates and serialize recipe IDs into `WarboundData.recipes`.

## Draft: Implement Toy Collection Snapshot
### Agent
Track toy unlocks and encode them into `WarboundData.toys`. Use Blizzard APIs to fetch unlock state.

## Draft: Implement Real-Time Update System
### Agent
Implement a lightweight event dispatcher that listens for collection updates and rebuilds only the changed category to minimize overhead.

## Draft: Capture Vendor Metadata
### Agent
Hook into merchant windows and serialize available item IDs, including cost, currency, and reputation requirements.

## Draft: Capture Quest Reward Sources
### Agent
Capture which items appear as quest rewards. Hook into quest accept/turn-in events and store relevant metadata.

## Draft: Implement World Drop Attribution
### Agent
Infer world drop context from open-world kills and loot. Store zone and NPC info where possible.

## Draft: Capture Zone & Map Context
### Agent
Track the player’s zone, subzone, and map ID as context when collecting items. Log only if new relevant data is detected.

## Draft: Capture NPC Interaction Metadata
### Agent
Capture NPC ID, name, location, and faction on interaction. Store lightweight metadata for later use.

## Draft: Create Dedicated Chat Channel
### Agent
Register and route addon output to a clean, user-facing chat channel. Default to off unless explicitly enabled by user.

## Draft: Post-Instance Collection Summary
### Agent
After a dungeon or raid, show a summary of new appearances, mounts, pets, etc., earned during the run.

## Draft: Goal Completion Notifications
### Agent
Display celebratory messages when a full goal is completed. Include simple message and point summary.

## Draft: Build LUA Folder Publishing Utility
### Agent
Create a C# utility to copy addon files from a `Dev/` folder to the actual WoW AddOns directory.
