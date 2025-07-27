# Plan: Recipe Reagent Tracking

## Intent  
Create a RecipeIngredientsETL to identify and track all item IDs used as reagents in recipes, ensuring these commodity items are included in auction price tracking without requiring manual item configuration.

## Context  
The existing auction ETL successfully tracks item prices, but only processes items already present in the item tables. Recipe reagents (primarily commodities like linen cloth) are not automatically tracked since their item IDs aren't in the database. The recipe endpoint already captures reagent data in a semicolon-delimited format ("itemId:quantity;itemId:quantity;"), providing the foundation for automated reagent discovery. Following the established ItemIndexETL pattern, this ETL will create item stubs that the existing ItemETL can later enrich.

## In Scope  
- Create RecipeIngredientsETL following RunnableBlizzardETL pattern to process complete recipes
- Parse semicolon-delimited reagent strings to extract item IDs and quantities 
- Create Item records with NEEDS_ENRICHED status for reagent items not currently in database
- Integrate with existing ETL pipeline so ItemETL and AuctionETL automatically process discovered reagents
- Follow established ETL patterns for error handling and status management

## Acceptance Criteria  
- Given recipes with Status COMPLETE containing reagent data, when RecipeIngredientsETL processes them, then missing reagent item IDs are added to Items table with NEEDS_ENRICHED status
- Given reagent string "12345:2;67890:5;", when parsed by RecipeIngredientsETL, then item IDs 12345 and 67890 are created as Item records if they don't exist
- Given newly created reagent Item records, when ItemETL runs, then reagent items are enriched with Blizzard API data
- Given enriched reagent items, when AuctionETL runs, then reagent commodity prices are tracked in auction data