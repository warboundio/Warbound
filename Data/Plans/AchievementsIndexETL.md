# Plan: AchievementsIndexETL Implementation

## Intent  
Add the foundational infrastructure to collect and stub all achievement data from Blizzard's API, creating the baseline for future achievement tracking features in Warbound.

## Context  
Following the established ETL pattern used for other collectible items (ItemAppearanceIndexETL, MountIndexETL, etc.), we need to create the achievement data collection pipeline. This builds on the existing RunnableBlizzardETL framework and BlizzardAPI infrastructure to expand Warbound's collectible tracking beyond transmog, mounts, and pets to include achievements.

## In Scope  
- Create Achievement database entity with appropriate schema and constraints
- Implement AchievementIndexEndpoint to call Blizzard's achievement index API
- Build AchievementsIndexETL following the established ETL pattern to stub achievement data
- Update BlizzardAPIContext to include Achievement DbSet
- Update WarcraftData singleton to include Achievement dictionary
- Set achievement status to NEEDS_ENRICHED for future enrichment pipeline

## Acceptance Criteria  
- Given the AchievementsIndexETL runs, when it processes the Blizzard achievement index API, then it creates Achievement entities with Id and Name populated and Status set to NEEDS_ENRICHED
- Given an Achievement entity is created, when saved to database, then it has sensible defaults (empty strings for text, -1 for integers, proper MaxLength constraints)
- Given the ETL follows existing patterns, when compared to ItemAppearanceIndexETL, then it uses the same architectural approach and error handling philosophy