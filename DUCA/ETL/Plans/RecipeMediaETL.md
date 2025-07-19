# DUCA Plan: Create Recipe Media Enrichment ETL
This plan will implement a new enrichment ETL that retrieves World of Warcraft Recipe Media data from Blizzard's Developer API for existing recipe records.

## 1. Read Endpoint Instructions
- Open and follow **ETL/BlizzardAPI/Endpoints/_Endpoints.Agent.md** to understand how to create an enrichment endpoint for detailed Recipe Media data.
- Review existing media endpoints like **ItemMediaEndpoint.cs** for established patterns.

## 2. Read ETL Instructions
- Open and follow **ETL/ETLs/_ETLs.Agent.md** to understand how to create an enrichment ETL class that processes existing Recipe records.
- Review **ItemMediaETL.cs** as a reference for media enrichment ETL implementation patterns.

## 3. Create Recipe Media Domain Model
Create **ETL/BlizzardAPI/Endpoints/RecipeMedia.cs** following these requirements:
- Include `[Table("recipe_media", Schema = "wow")]` attribute
- Required properties:
  - `Id` (int, primary key) - matches recipe ID
  - `URL` (string, MaxLength 1023) - icon URL from API response
  - `Status` (ETLStateType) - default NEEDS_ENRICHED
  - `LastUpdatedUtc` (DateTime) - default DateTime.UtcNow
- Follow existing naming conventions with appropriate default values (-1 for integers, string.Empty for strings)

## 4. Create Recipe Media Enrichment Endpoint
Create **ETL/BlizzardAPI/Endpoints/RecipeMediaEndpoint.cs** following these requirements:
- Inherit from `BaseBlizzardEndpoint<RecipeMedia>`
- Require `recipeId` parameter in constructor
- Implement fail-fast parsing pattern (individual property assignments, no object initializers)
- Parse all fields from JSON response following ItemMediaEndpoint pattern
- Set `Status = ETLStateType.COMPLETE` after successful parsing
- Use non-null assertion (`!`) for required string properties
- Handle the assets array and extract the first asset's "value" property as URL

## 5. Create Recipe Media Enrichment ETL
Create **ETL/ETLs/RecipeMediaETL.cs** following these requirements:
- Inherit from `RunnableBlizzardETL`
- Include required `RunAsync` entry point signature
- `GetItemsToProcessAsync`: Query existing Recipe records that don't have corresponding RecipeMedia entries
- `UpdateItemAsync`: Use `RecipeMediaEndpoint` to fetch media data and create new RecipeMedia records
- Set `Status = ETLStateType.COMPLETE` and update `LastUpdatedUtc`
- Add new RecipeMedia objects to `SaveBuffer`

## 6. Create Unit Tests
Create **ETL/BlizzardAPI/Endpoints/RecipeMediaEndpointTests.cs** following established patterns:
- Test method name: `ItShouldParseRecipeMediaData`
- Use existing **RecipeMedia.json** fixture for test data
- Assert all fields are parsed correctly from JSON (Id, URL, Status)
- Verify `Status` is set to `COMPLETE`
- Follow existing endpoint test patterns from **ItemMediaEndpointTests.cs**

## 7. Update Database Context
Add the new RecipeMedia entity to **ETL/BlizzardAPI/BlizzardAPIContext.cs**:
- Include `DbSet<RecipeMedia> RecipeMedias { get; set; }` property
- Follow existing naming conventions

## Implementation Details

### API Endpoint
- **URL**: `https://us.api.blizzard.com/data/wow/media/recipe/{recipeId}?namespace=static-us&locale=en_US`
- **Method**: GET
- **Response**: Individual Recipe Media details (see RecipeMedia.json for structure)

### JSON Field Mapping
- `id` → `Id` (recipe ID)
- `assets[0].value` → `URL` (icon URL string)
- Set `Status` to `COMPLETE` after successful parsing
- Set `LastUpdatedUtc` to current time

### Parsing Considerations
- Follow ItemMediaEndpoint pattern for parsing assets array
- Extract the first asset's "value" property as the icon URL
- Use fail-fast parsing - let missing required fields throw immediately
- Apply individual property assignment pattern for all fields

### ETL Logic
- Query `Context.Recipes` to get all existing recipe IDs
- Query `Context.RecipeMedias` to get existing recipe media IDs
- Process recipes that exist but don't have corresponding media records
- Avoid duplicates by checking existing RecipeMedia IDs before processing

### Success Criteria
- RecipeMedia domain model includes all 4 required properties with correct data types
- RecipeMediaEndpoint correctly parses all fields from RecipeMedia.json fixture
- RecipeMediaETL successfully creates RecipeMedia records for existing recipes without duplicates
- All unit tests pass
- Code follows established fail-fast and individual assignment patterns
- Database context includes RecipeMedia entity
- No duplicate RecipeMedia records are created for the same recipe

### Out of Scope
- Do not create database migrations
- Do not modify existing RecipeIndexETL functionality
- Do not change existing RecipeMedia.json fixture
- Do not modify existing Recipe model or related functionality