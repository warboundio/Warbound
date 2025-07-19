# DUCA Plan: Create Profession Media Enrichment ETL
This plan will implement a new enrichment ETL that retrieves World of Warcraft Profession Media data from Blizzard's Developer API for existing profession records.

## 1. Read Endpoint Instructions
- Open and follow **ETL/BlizzardAPI/Endpoints/_Endpoints.Agent.md** to understand how to create an enrichment endpoint for detailed Profession Media data.
- Review existing media endpoints like **ItemMediaEndpoint.cs** for established patterns.

## 2. Read ETL Instructions
- Open and follow **ETL/ETLs/_ETLs.Agent.md** to understand how to create an enrichment ETL class that processes existing Profession records.
- Review **ItemMediaETL.cs** as a reference for media enrichment ETL implementation patterns.

## 3. Create Profession Media Domain Model
Create **ETL/BlizzardAPI/Endpoints/ProfessionMedia.cs** following these requirements:
- Include `[Table("profession_media", Schema = "wow")]` attribute
- Required properties:
  - `Id` (int, primary key) - matches profession ID
  - `URL` (string, MaxLength 1023) - icon URL from API response
  - `Status` (ETLStateType) - default NEEDS_ENRICHED
  - `LastUpdatedUtc` (DateTime) - default DateTime.UtcNow
- Follow existing naming conventions with appropriate default values (-1 for integers, string.Empty for strings)

## 4. Create Profession Media Enrichment Endpoint
Create **ETL/BlizzardAPI/Endpoints/ProfessionMediaEndpoint.cs** following these requirements:
- Inherit from `BaseBlizzardEndpoint<ProfessionMedia>`
- Require `professionId` parameter in constructor
- Implement fail-fast parsing pattern (individual property assignments, no object initializers)
- Parse all fields from JSON response following ItemMediaEndpoint pattern
- Set `Status = ETLStateType.COMPLETE` after successful parsing
- Use non-null assertion (`!`) for required string properties
- Handle the assets array and extract the first asset's "value" property as URL

## 5. Create Profession Media Enrichment ETL
Create **ETL/ETLs/ProfessionMediaETL.cs** following these requirements:
- Inherit from `RunnableBlizzardETL`
- Include required `RunAsync` entry point signature
- `GetItemsToProcessAsync`: Query existing Profession records that don't have corresponding ProfessionMedia entries
- `UpdateItemAsync`: Use `ProfessionMediaEndpoint` to fetch media data and create new ProfessionMedia records
- Set `Status = ETLStateType.COMPLETE` and update `LastUpdatedUtc`
- Add new ProfessionMedia objects to `SaveBuffer`

## 6. Create Unit Tests
Create **ETL/BlizzardAPI/Endpoints/ProfessionMediaEndpointTests.cs** following established patterns:
- Test method name: `ItShouldParseProfessionMediaData`
- Use existing **ProfessionMedia.json** fixture for test data
- Assert all fields are parsed correctly from JSON (Id, URL, Status)
- Verify `Status` is set to `COMPLETE`
- Follow existing endpoint test patterns from **ItemMediaEndpointTests.cs**

## 7. Update Database Context
Add the new ProfessionMedia entity to **ETL/BlizzardAPI/BlizzardAPIContext.cs**:
- Include `DbSet<ProfessionMedia> ProfessionMedias { get; set; }` property
- Follow existing naming conventions

## Implementation Details

### API Endpoint
- **URL**: `https://us.api.blizzard.com/data/wow/media/profession/{professionId}?namespace=static-us&locale=en_US`
- **Method**: GET
- **Response**: Individual Profession Media details (see ProfessionMedia.json for structure)

### JSON Field Mapping
- `id` → `Id` (profession ID)
- `assets[0].value` → `URL` (icon URL string)
- Set `Status` to `COMPLETE` after successful parsing
- Set `LastUpdatedUtc` to current time

### Parsing Considerations
- Follow ItemMediaEndpoint pattern for parsing assets array
- Extract the first asset's "value" property as the icon URL
- Use fail-fast parsing - let missing required fields throw immediately
- Apply individual property assignment pattern for all fields

### ETL Logic
- Query `Context.Professions` to get all existing profession IDs
- Query `Context.ProfessionMedias` to get existing profession media IDs
- Process professions that exist but don't have corresponding media records
- Avoid duplicates by checking existing ProfessionMedia IDs before processing

### Success Criteria
- ProfessionMedia domain model includes all 4 required properties with correct data types
- ProfessionMediaEndpoint correctly parses all fields from ProfessionMedia.json fixture
- ProfessionMediaETL successfully creates ProfessionMedia records for existing professions without duplicates
- All unit tests pass
- Code follows established fail-fast and individual assignment patterns
- Database context includes ProfessionMedia entity
- No duplicate ProfessionMedia records are created for the same profession

### Out of Scope
- Do not create database migrations
- Do not modify existing ProfessionIndexETL functionality
- Do not change existing ProfessionMedia.json fixture
- Do not modify existing Profession model or related functionality