# Plan: Implement AchievementMediaETL

## Intent  
Add AchievementMediaETL to fetch and store achievement icon URLs from Blizzard API, enabling achievement image display in the UI.

## Context  
Building on existing Achievement data from AchievementsIndexETL and following established media patterns from ItemMediaETL. The Blizzard API provides achievement media at `/data/wow/media/achievement/{id}` with icon URLs in the assets array.

## In Scope  
- Create AchievementMedia entity class following ItemMedia pattern
- Implement AchievementMediaEndpoint for API communication  
- Build AchievementMediaETL to process existing achievements
- Add database integration to BlizzardAPIContext and WarcraftData
- Remove corresponding draft from Drafts.md

## Acceptance Criteria  
- Given achievements exist in database, when AchievementMediaETL runs, then achievement media records are created with id and URL fields
- Given an achievement ID, when AchievementMediaEndpoint is called, then it returns parsed AchievementMedia with icon URL from assets.value
- Given AchievementMediaETL completes, then ETLStatus is set to COMPLETE and no duplicate records exist
- Given new code follows existing patterns, then AchievementMedia uses same structure as ItemMedia with appropriate string length limits