# DUCA Plan: Create Recipe ETL
This plan will extend the existing Recipe tracking system by implementing an enrichment ETL that retrieves detailed World of Warcraft Recipe data from Blizzard's Developer API. This ETL will process Recipe records that have `Status = NEEDS_ENRICHED` and populate additional fields including crafted item details and reagent requirements.

## 1. Read Endpoint Instructions
- Open and follow **ETL/BlizzardAPI/Endpoints/_Endpoints.Agent.md** to understand how to create an enrichment endpoint.
- Review existing enrichment endpoints like **ItemEndpoint.cs** and **PetEndpoint.cs** for established patterns.

## 2. Read ETL Instructions
- Open and follow **ETL/ETLs/_ETLs.Agent.md** to understand how to create an enrichment ETL class.
- Review **ItemETL.cs** and **PetETL.cs** as references for enrichment ETL implementation patterns.

## 3. Extend Recipe Domain Model
Update **ETL/BlizzardAPI/Endpoints/Recipe.cs** by adding these new properties:
- `CraftedItemId` (int) with default value of -1
- `CraftedQuantity` (int) with default value of -1  
- `Reagents` (string) with `[MaxLength(2047)]` attribute and default value of string.Empty

The `Reagents` field will store a semicolon-delimited string in the format: `{itemId}:{quantity};{itemId}:{quantity};`
Example: `2835:1;` means item ID 2835 with quantity 1 is required.

## 4. Create Recipe Enrichment Endpoint
Create **ETL/BlizzardAPI/Endpoints/RecipeEndpoint.cs** following these requirements:
- Inherit from `BaseBlizzardEndpoint<Recipe>`
- Require `recipeId` parameter in constructor
- Implement fail-fast parsing pattern (individual property assignments, no object initializers)
- Parse recipe data from the individual recipe JSON response
- For the Recipe object, parse:
  - `Id` from recipe id
  - `Name` from recipe name
  - `CraftedItemId` from `crafted_item.id`
  - `CraftedQuantity` from `crafted_quantity.value`
  - `Reagents` by iterating through `reagents[]` array and building string format `{reagent.id}:{quantity};`
  - `Status = ETLStateType.COMPLETE`
  - `LastUpdatedUtc = DateTime.UtcNow`

## 5. Create Recipe Enrichment ETL
Create **ETL/ETLs/RecipeETL.cs** following these requirements:
- Inherit from `RunnableBlizzardETL`
- Include required `RunAsync` entry point signature
- `GetItemsToProcessAsync`: Query Recipe records with `Status = NEEDS_ENRICHED`
  - Return Recipe objects that need enrichment
- `UpdateItemAsync`: Use `RecipeEndpoint` to fetch detailed recipe data and update Recipe record
  - Cast item to Recipe object
  - Call RecipeEndpoint with Recipe.Id
  - Update the existing Recipe record with enriched data
  - Add updated Recipe to `SaveBuffer`

## 6. Create Unit Tests
Create **ETL/BlizzardAPI/Endpoints/RecipeEndpointTests.cs** following established patterns:
- Test method name: `ItShouldParseRecipeData`
- Use existing **Recipe.json** fixture for test data
- Assert all fields are parsed correctly:
  - Verify `Id` and `Name` are set
  - Verify `CraftedItemId` is 2862 (from fixture)
  - Verify `CraftedQuantity` is 1 (from fixture)
  - Verify `Reagents` is "2835:1;" (from fixture reagents array)
  - Verify `Status` is `ETLStateType.COMPLETE`
- Follow existing endpoint test patterns from **ItemEndpointTests.cs**

## Implementation Details

### API Endpoint
- **URL**: `https://us.api.blizzard.com/data/wow/recipe/{recipeId}?namespace=static-us&locale=en_US`
- **Method**: GET
- **Response**: Individual recipe details (see Recipe.json for structure)

### JSON Field Mapping
- `id` → Recipe `Id`
- `name` → Recipe `Name`
- `crafted_item.id` → Recipe `CraftedItemId`
- `crafted_quantity.value` → Recipe `CraftedQuantity`
- `reagents[]` → Recipe `Reagents` (serialized as string)

### Reagents String Format
- Parse the `reagents[]` array from JSON
- For each reagent object:
  - Extract `reagent.id` and `quantity`
  - Format as `{id}:{quantity};`
  - Concatenate all reagents into single string
- Example: `[{"reagent":{"id":2835},"quantity":1}]` becomes `"2835:1;"`
- Handle empty reagents array by setting to empty string

### Parsing Considerations
- Use fail-fast parsing - let missing required fields throw immediately
- Apply individual property assignment pattern for all fields
- Handle cases where reagents array might be empty
- Ensure reagents string doesn't exceed 2047 character limit

### ETL Processing Logic
- Query Recipe table for records with `Status = NEEDS_ENRICHED`
- For each recipe, call RecipeEndpoint to get detailed data
- Update the existing Recipe record with enriched fields
- Set `Status = COMPLETE` after successful enrichment
- This ensures each recipe is only enriched once

### Success Criteria
- Recipe domain model includes new properties with correct data types and constraints
- RecipeEndpoint correctly parses all fields from Recipe.json fixture
- Reagents string is properly formatted with semicolon delimiters
- RecipeETL successfully enriches Recipe records with detailed data
- All unit tests pass
- Code follows established fail-fast and individual assignment patterns
- ETL is idempotent and can be run multiple times safely

### Out of Scope
- Do not create database migrations
- Do not modify existing RecipeIndexETL functionality
- Do not parse media or other complex nested objects beyond crafted_item and reagents
- Do not change existing Recipe.json fixture
- Do not implement any UI or reporting features