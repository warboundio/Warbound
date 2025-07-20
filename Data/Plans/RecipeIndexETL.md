# DUCA Plan: Create Recipe Index ETL

Extract Recipe Index data from profession skill tiers. Create Recipe records for future enrichment.

**Context:** Follow **ETL/BlizzardAPI/Endpoints/_Endpoints.Agent.md** and **ETL/ETLs/_ETLs.Agent.md**

## Domain Model
Create `Recipe.cs` with:
- Standard ETL properties: `Id`, `Name`, `Status`, `LastUpdatedUtc`
- Profession tracking: `ProfessionId`, `SkillTierId`
- Use `[Table("recipe", Schema = "wow")]`

## Key Implementation

**RecipeIndexEndpoint.cs:** Parse `categories[].recipes[]` from skill tier response. Set `Status = NEEDS_ENRICHED`.

**RecipeIndexETL.cs:** Query existing Profession records, split `SkillTiers` semicolon-delimited field, process combinations without existing recipes.

## API Details
- **URL:** `/data/wow/profession/{professionId}/skill-tier/{skillTierId}`
- **Parse:** Extract all recipes from categories array
- **Mapping:** recipe id/name + constructor professionId/skillTierId

## Testing
- Use existing `ProfessionSkillTier.json` fixture
- Verify all recipes parsed from categories
- Test profession/skill tier ID assignment

## Database
Add `DbSet<Recipe> Recipes` to BlizzardAPIContext.cs