# DUCA Plan: Create Recipe ETL

Enrich Recipe records with detailed data from Blizzard API. Target records with `Status = NEEDS_ENRICHED`.

**Context:** Follow **ETL/BlizzardAPI/Endpoints/_Endpoints.Agent.md** and **ETL/ETLs/_ETLs.Agent.md**

## Domain Model Updates

Extend `Recipe.cs` with:
- `CraftedItemId` (int, default -1)
- `CraftedQuantity` (int, default -1)  
- `Reagents` (string, MaxLength 2047, default empty)

**Format:** `Reagents` as semicolon-delimited: `{itemId}:{quantity};{itemId}:{quantity};`

## Key Deviations

**RecipeEndpoint.cs:** Parse `reagents[]` array into custom string format. Handle empty arrays gracefully.

**RecipeETL.cs:** Update existing records (not create new). Set `Status = COMPLETE` after enrichment.

## API Details
- **URL:** `/data/wow/recipe/{recipeId}`
- **Parse:** `crafted_item.id` → `CraftedItemId`, `crafted_item.crafted_quantity` → `CraftedQuantity`
- **Reagents:** Transform `[{"reagent":{"id":2835},"quantity":1}]` → `"2835:1;"`

## Testing
- Use existing Recipe.json fixture
- Test reagents string formatting
- Verify empty reagents handling