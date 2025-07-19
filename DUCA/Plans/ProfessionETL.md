# DUCA Plan: Create Profession Enrichment ETL
This plan will extend the existing Profession domain model and implement an enrichment ETL that retrieves detailed World of Warcraft Profession data from Blizzard's Developer API.

## 1. Read Endpoint Instructions
- Open and follow **ETL/BlizzardAPI/Endpoints/_Endpoints.Agent.md** to understand how to create an enrichment endpoint for detailed Profession data.
- Review existing enrichment endpoints like **MountEndpoint.cs** and **ToyEndpoint.cs** for established patterns.

## 2. Read ETL Instructions
- Open and follow **ETL/ETLs/_ETLs.Agent.md** to understand how to create an enrichment ETL class that processes existing Profession records.
- Review **MountETL.cs** as a reference for enrichment ETL implementation patterns.

## 3. Extend Profession Domain Model
Update **ETL/BlizzardAPI/Endpoints/Profession.cs** to include the following additional properties listed below in implementation details.
All new fields should follow existing naming conventions and include appropriate default values (-1 for integers, string.Empty for strings, false for booleans).

## 4. Create Profession Enrichment Endpoint
Create **ETL/BlizzardAPI/Endpoints/ProfessionEndpoint.cs** following these requirements:
- Inherit from `BaseBlizzardEndpoint<Profession>`
- Require `professionId` parameter in constructor
- Implement fail-fast parsing pattern (individual property assignments, no object initializers)
- Parse all new fields from JSON response
- Set `Status = ETLStateType.COMPLETE` after successful parsing
- Use non-null assertion (`!`) for required string properties
- Handle optional/missing JSON properties appropriately

## 5. Create Profession Enrichment ETL
Create **ETL/ETLs/ProfessionETL.cs** following these requirements:
- Inherit from `RunnableBlizzardETL`
- Include required `RunAsync` entry point signature
- `GetItemsToProcessAsync`: Query existing Profession records with `Status == NEEDS_ENRICHED`
- `UpdateItemAsync`: Use `ProfessionEndpoint` to fetch detailed data and update all new fields
- Set `Status = ETLStateType.COMPLETE` and update `LastUpdatedUtc`
- Add updated Profession objects to `SaveBuffer`

## 6. Create Unit Tests
Create **ETL/BlizzardAPI/Endpoints/ProfessionEndpointTests.cs** following established patterns:
- Test method name: `ItShouldParseProfessionData`
- Use existing **Profession.json** fixture for test data
- Assert all new fields are parsed correctly from JSON
- Verify `Status` is set to `COMPLETE`
- Follow existing endpoint test patterns from **MountEndpointTests.cs**

## 7. Database Context
The Profession entity is already included in **ETL/BlizzardAPI/BlizzardAPIContext.cs** with the `DbSet<Profession> Professions` property - no changes needed.

## Implementation Details

### API Endpoint
- **URL**: `https://us.api.blizzard.com/data/wow/profession/{professionId}?namespace=static-us&locale=en_US`
- **Method**: GET
- **Response**: Individual Profession details (see Profession.json for structure)

### JSON Field Mapping
- `type.type` → `Type` (e.g., "PRIMARY")
- `skill_tiers[].id` → `SkillTiers` (semicolon-delimited string of IDs, e.g., "2437;2454;2472;2473;2474;2475;2476;2477;2751;2822;2872")
- `name` → `Name` (populate if not already set from index ETL)

### Parsing Considerations
- Skip complex nested objects like `media` - not required for this implementation
- Special handling for `skill_tiers` array: extract only the `id` values and join with semicolons
- Use fail-fast parsing - let missing required fields throw immediately
- Apply individual property assignment pattern for all fields
- Handle cases where `skill_tiers` array might be empty (set to empty string)

### Success Criteria
- Profession domain model includes new `Type` and `SkillTiers` properties with correct data types
- ProfessionEndpoint correctly parses all fields from Profession.json fixture
- ProfessionETL successfully enriches existing Profession records from NEEDS_ENRICHED to COMPLETE
- All unit tests pass
- Code follows established fail-fast and individual assignment patterns
- Database context includes Profession entity
- This completes the profession enrichment process - objects are marked as COMPLETE

### Out of Scope
- Do not create database migrations
- Do not modify existing ProfessionIndexETL functionality
- Do not implement parsing of complex nested objects like `media` or `description`
- Do not change existing Profession.json fixture