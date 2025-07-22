# Plan: Attach Addon Objects to Database

## Intent  
Convert existing Data/Addon objects to follow BlizzardAPI endpoint patterns and integrate them into the database context with generated table prefixes.

## Context  
Data/Addon contains five objects (LootLogEntry, NPCKillCount, PetBattleLocation, Vendor, VendorItem) collected from the addon that need database integration. These objects should follow the established BlizzardAPI endpoint structure for consistency and future ETL processing.

## In Scope  
- Convert all five Data/Addon objects to follow BlizzardAPI endpoint patterns with table attributes, ETL state tracking, and timestamps
- Prefix all table names with 'g_' to denote generated content  
- Add appropriate MaxLength attributes based on data analysis
- Integrate objects into BlizzardAPIContext with 'G_' prefixed property names
- Add objects to WarcraftData singleton for caching

## Acceptance Criteria  
- Given existing addon objects, when converted to database entities, then they include [Table] attributes with 'g_' prefixed names and "wow" schema
- Given converted objects, when added to BlizzardAPIContext, then DbSet properties use 'G_' prefix naming convention
- Given objects with string properties, when MaxLength is applied, then values are appropriate for expected data size
- (DEVELOPER NOTE: NO. These are not ETL and do not need Status and LastUpdatedUtc. As of now let's leave off LastUpdatedUtc given the sheer volume of rows we'll be writing - we'll revisit that later - the instruction is to not add *any* properties. 
