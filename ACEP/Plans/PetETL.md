# ACEP Plan: Create Pet Enrichment ETL
This plan will extend the existing Pet domain model and implement an enrichment ETL that retrieves detailed World of Warcraft Pet data from Blizzard's Developer API.

## 1. Read Endpoint Instructions
- Open and follow **ETL/BlizzardAPI/Endpoints/_Endpoints.Agent.md** to understand how to create an enrichment endpoint for detailed Pet data.
- Review existing enrichment endpoints like **MountEndpoint.cs** and **ToyEndpoint.cs** for established patterns.

## 2. Read ETL Instructions
- Open and follow **ETL/ETLs/_ETLs.Agent.md** to understand how to create an enrichment ETL class that processes existing Pet records.
- Review **MountETL.cs** as a reference for enrichment ETL implementation patterns.

## 3. Extend Pet Domain Model
Update **ETL/BlizzardAPI/Endpoints/Pet.cs** to include the following additional properties listed below in implementation details.
All new fields should follow existing naming conventions and include appropriate default values (-1 for integers, string.Empty for strings, false for booleans).

## 4. Create Pet Enrichment Endpoint
Create **ETL/BlizzardAPI/Endpoints/PetEndpoint.cs** following these requirements:
- Inherit from `BaseBlizzardEndpoint<Pet>`
- Require `petId` parameter in constructor
- Implement fail-fast parsing pattern (individual property assignments, no object initializers)
- Parse all new fields from JSON response
- Set `Status = ETLStateType.COMPLETE` after successful parsing
- Use non-null assertion (`!`) for required string properties
- Handle optional/missing JSON properties appropriately

## 5. Create Pet Enrichment ETL
Create **ETL/ETLs/PetETL.cs** following these requirements:
- Inherit from `RunnableBlizzardETL`
- Include required `RunAsync` entry point signature
- `GetItemsToProcessAsync`: Query existing Pet records with `Status == NEEDS_ENRICHED`
- `UpdateItemAsync`: Use `PetEndpoint` to fetch detailed data and update all new fields
- Set `Status = ETLStateType.COMPLETE` and update `LastUpdatedUtc`
- Add updated Pet objects to `SaveBuffer`

## 6. Create Unit Tests
Create **ETL/BlizzardAPI/Endpoints/PetEndpointTests.cs** following established patterns:
- Test method name: `ItShouldParsePetData`
- Use existing **Pet.json** fixture for test data
- Assert all new fields are parsed correctly from JSON
- Verify `Status` is set to `COMPLETE`
- Follow existing endpoint test patterns from **MountEndpointTests.cs**

## 7. Update Database Context
Add the new Pet entity to **ETL/BlizzardAPI/BlizzardAPIContext.cs**:
- Include `DbSet<Pet> Pets { get; set; }` property
- Follow existing naming conventions

## Implementation Details

### API Endpoint
- **URL**: `https://us.api.blizzard.com/data/wow/pet/{petId}?namespace=static-us&locale=en_US`
- **Method**: GET
- **Response**: Individual Pet details (see Pet.json for structure)

### JSON Field Mapping
- `battle_pet_type.type` → `BattlePetType` (e.g., "MECHANICAL")
- `is_capturable` → `IsCapturable` (boolean)
- `is_tradable` → `IsTradable` (boolean)
- `is_battlepet` → `IsBattlePet` (boolean)
- `is_alliance_only` → `IsAllianceOnly` (boolean)
- `is_horde_only` → `IsHordeOnly` (boolean)
- `source.type` → `SourceType` (e.g., "PROFESSION")
- `icon` → `Icon` (full URL string)

### Parsing Considerations
- Skip `abilities` array - not required for this implementation
- Special Exception: Handle missing or null `source` object gracefully. This may not always be populated in the API repsonse and that's OK.
- Use fail-fast parsing - let missing required fields throw immediately
- Apply individual property assignment pattern for all fields

### Success Criteria
- Pet domain model includes all 8 new properties with correct data types
- PetEndpoint correctly parses all fields from Pet.json fixture
- PetETL successfully enriches existing Pet records from NEEDS_ENRICHED to COMPLETE
- All unit tests pass
- Code follows established fail-fast and individual assignment patterns
- Database context includes Pet entity

### Out of Scope
- Do not create database migrations
- Do not modify existing PetIndexETL functionality
- Do not implement abilities parsing
- Do not change existing Pet.json fixture