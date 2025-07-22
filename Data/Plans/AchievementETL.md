# Plan: Implement AchievementETL

## Intent  
Create AchievementETL to enrich achievement data from AchievementsIndexETL by calling the Blizzard API and adding detailed achievement information including description, category, rewards, points, icon, and criteria data.

## Context  
Building on the existing AchievementsIndexETL that creates baseline Achievement records with NEEDS_ENRICHED status. This follows established patterns from ItemAppearanceETL and MountETL for data enrichment. The Blizzard Achievement API provides rich metadata that will enhance the achievement tracking capabilities of Warbound.

## In Scope  
- Create AchievementEndpoint to call Blizzard API (https://us.api.blizzard.com/data/wow/achievement/{id}?namespace=static-us&locale=en_US)
- Update Achievement model to include new fields: Description, CategoryName, RewardDescription, RewardItemId, RewardItemName, Points, Icon, CriteriaIds, CriteriaTypes
- Create AchievementETL class following existing ETL patterns to process NEEDS_ENRICHED achievements
- Handle optional reward fields (reward_description, reward_item.id, reward_item.name) with sensible defaults
- Remove corresponding draft from Drafts.md
- Use appropriate MaxLength attributes (2047 for descriptions, 255 for names)

## Acceptance Criteria  
- Given achievements exist with NEEDS_ENRICHED status, when AchievementETL runs, then it calls the Blizzard API for each achievement
- Given a successful API response, when processing achievement data, then all available fields are populated including semicolon-delimited criteria lists
- Given optional reward fields are missing, when parsing the response, then empty strings are used as defaults
- Given successful data enrichment, when the ETL completes, then achievement status is set to COMPLETE
- Given the draft exists in Drafts.md, when the plan is created, then the corresponding draft is removed