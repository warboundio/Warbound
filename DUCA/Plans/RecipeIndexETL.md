# DUCA Plan: Create Recipe Index ETL
This plan will extend the recipe tracking system by implementing an ETL that retrieves World of Warcraft Recipe Index data from Blizzard's Developer API. For each combination of profession and skill tier, the ETL will extract all recipes and create index records for future enrichment.

## 1. Read Endpoint Instructions
- Open and follow **ETL/BlizzardAPI/Endpoints/_Endpoints.Agent.md** to understand how to create an endpoint for skill tier data.
- Review existing endpoints like **ProfessionIndexEndpoint.cs** and **PetIndexEndpoint.cs** for established patterns.

## 2. Read ETL Instructions
- Open and follow **ETL/ETLs/_ETLs.Agent.md** to understand how to create an index ETL class that processes existing data.
- Review **ProfessionIndexETL.cs** and **PetIndexETL.cs** as references for index ETL implementation patterns.

## 3. Create Recipe Domain Model
Create **ETL/BlizzardAPI/Endpoints/Recipe.cs** following these requirements:
- Include standard ETL properties: `Id`, `Name`, `Status`, `LastUpdatedUtc`
- Add profession tracking properties: `ProfessionId`, `SkillTierId` 
- Follow existing naming conventions and include appropriate default values (-1 for integers, string.Empty for strings)
- Use `[Table("recipe", Schema = "wow")]` attribute for database mapping

## 4. Create Recipe Index Endpoint
Create **ETL/BlizzardAPI/Endpoints/RecipeIndexEndpoint.cs** following these requirements:
- Inherit from `BaseBlizzardEndpoint<List<Recipe>>`
- Require `professionId` and `skillTierId` parameters in constructor
- Implement fail-fast parsing pattern (individual property assignments, no object initializers)
- Parse recipe data from skill tier JSON response structure
- Extract recipes from `categories[].recipes[]` array in the JSON
- For each recipe, create Recipe object with:
  - `Id` from recipe id
  - `Name` from recipe name
  - `ProfessionId` from constructor parameter
  - `SkillTierId` from constructor parameter
  - `Status = ETLStateType.NEEDS_ENRICHED`
  - `LastUpdatedUtc = DateTime.UtcNow`

## 5. Create Recipe Index ETL
Create **ETL/ETLs/RecipeIndexETL.cs** following these requirements:
- Inherit from `RunnableBlizzardETL`
- Include required `RunAsync` entry point signature
- `GetItemsToProcessAsync`: Query existing Profession records to get profession/skill tier combinations
  - Parse `SkillTiers` semicolon-delimited field from Profession table
  - Return tuples of (professionId, skillTierId) that don't have recipes yet
- `UpdateItemAsync`: Use `RecipeIndexEndpoint` to fetch skill tier data and create Recipe records
  - Cast item to profession/skill tier pair
  - Call RecipeIndexEndpoint with professionId and skillTierId
  - Add all returned Recipe objects to `SaveBuffer`

## 6. Create Unit Tests
Create **ETL/BlizzardAPI/Endpoints/RecipeIndexEndpointTests.cs** following established patterns:
- Test method name: `ItShouldParseRecipeIndexData`
- Use existing **ProfessionSkillTier.json** fixture for test data
- Assert all recipes are parsed correctly from categories
- Verify profession and skill tier IDs are properly assigned
- Follow existing endpoint test patterns from **ProfessionIndexEndpointTests.cs**

## 7. Database Context
Update **ETL/BlizzardAPI/BlizzardAPIContext.cs** to include:
- `DbSet<Recipe> Recipes` property following existing conventions

## Implementation Details

### API Endpoint
- **URL**: `https://us.api.blizzard.com/data/wow/profession/{professionId}/skill-tier/{skillTierId}?namespace=static-us&locale=en_US`
- **Method**: GET
- **Response**: Skill tier details with categories containing recipes (see ProfessionSkillTier.json for structure)

### JSON Field Mapping
- `categories[].recipes[].id` → Recipe `Id`
- `categories[].recipes[].name` → Recipe `Name`
- Constructor `professionId` → Recipe `ProfessionId`
- Constructor `skillTierId` → Recipe `SkillTierId`

### Parsing Considerations
- Iterate through all categories in the skill tier response
- Extract all recipes from each category's recipes array
- Skip categories that don't have recipes (handle empty arrays)
- Use fail-fast parsing - let missing required fields throw immediately
- Apply individual property assignment pattern for all fields

### ETL Processing Logic
- Query Profession table for records with non-empty SkillTiers field
- Split SkillTiers on semicolon to get individual skill tier IDs
- For each profession/skill tier combination, check if recipes already exist
- Only process combinations that don't have existing recipe records
- This ensures idempotent ETL runs

### Success Criteria
- Recipe domain model includes all required properties with correct data types
- RecipeIndexEndpoint correctly parses all recipes from ProfessionSkillTier.json fixture
- RecipeIndexETL successfully creates Recipe records for all profession/skill tier combinations
- All unit tests pass
- Code follows established fail-fast and individual assignment patterns
- Database context includes Recipe entity
- ETL is idempotent and can be run multiple times safely

### Out of Scope
- Do not create database migrations
- Do not modify existing ProfessionETL or ProfessionIndexETL functionality
- Do not implement Recipe enrichment (that would be a separate RecipeETL plan)
- Do not parse complex nested objects like media or detailed reagent information
- Do not change existing ProfessionSkillTier.json fixture