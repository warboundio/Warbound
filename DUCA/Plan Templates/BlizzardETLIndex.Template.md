# DUCA Plan: Create {ResourceName} Index ETL

This plan will scaffold and implement a new ETL that retrieves the World of Warcraft {ResourceName} Index from Blizzard's Developer API.

## 1. Read Endpoint Instructions
- Open and follow **ETL/BlizzardAPI/Endpoints/_Endpoints.Agent.md** to understand how to define a new endpoint and its data model.
- Refer to existing endpoint examples like **ETL/BlizzardAPI/Endpoints/PetsIndex.json** for API return structure patterns.

## 2. Read ETL Instructions
- Open and follow **ETL/ETLs/_ETLs.Agent.md** to understand how to scaffold a new ETL class and the required method signatures.

## 3. Implementation Details

### API Endpoint
- **URL**: `{API_BASE_URL}/{resource}/index?namespace=static-us&locale=en_US`
- **Method**: GET
- **Response**: Index list containing {resource} entries

### C# Model Structure
Create **ETL/BlizzardAPI/Endpoints/{ResourceName}.cs** following these requirements:
- **SPECIFIC**: `Id` and `Name` of each {resource}
- **GENERAL**: Include standard ETL properties:
  - `Status = ETLStateType.NEEDS_ENRICHED` (default)
  - `LastUpdatedUtc = DateTime.UtcNow`
- Include any {resource}-specific properties as needed
- Use `[Table("{table_name}", Schema = "wow")]` attribute for database mapping
- Follow existing naming conventions and include appropriate default values (-1 for integers, string.Empty for strings)

### Endpoint Implementation
Create **ETL/BlizzardAPI/Endpoints/{ResourceName}IndexEndpoint.cs** following these requirements:
- Inherit from `BaseBlizzardEndpoint<List<{ResourceName}>>`
- Implement fail-fast parsing pattern (individual property assignments, no object initializers)
- Parse {resource} data from JSON response structure
- For each {resource}, create {ResourceName} object with:
  - `Id` from {resource} id
  - `Name` from {resource} name
  - Additional {resource}-specific fields as needed
  - `Status = ETLStateType.NEEDS_ENRICHED`
  - `LastUpdatedUtc = DateTime.UtcNow`

### ETL Implementation
Create **ETL/ETLs/{ResourceName}IndexETL.cs** following these requirements:
- Inherit from `RunnableBlizzardETL`
- Include required `RunAsync` entry point signature:
  ```csharp
  public static async Task RunAsync(ETLJob? job = null) 
      => await RunAsync<{ResourceName}IndexETL>(job);
  ```
- `GetItemsToProcessAsync`: Determine what {resource} records need to be created
  - Query existing {ResourceName} records to identify missing IDs
  - Return list of items that don't have {resource} records yet
- `UpdateItemAsync`: Use `{ResourceName}IndexEndpoint` to fetch index data and create {ResourceName} records
  - Call {ResourceName}IndexEndpoint to get {resource} data
  - Add all returned {ResourceName} objects to `SaveBuffer`

### Unit Tests
Create **ETL/BlizzardAPI/Endpoints/{ResourceName}IndexEndpointTests.cs** following established patterns:
- Test method name: `ItShouldParse{ResourceName}IndexData`
- Use appropriate JSON fixture for test data
- Assert all {resource} entries are parsed correctly
- Verify all required fields are properly assigned
- Follow existing endpoint test patterns

### Database Context
Update **ETL/BlizzardAPI/BlizzardAPIContext.cs** to include:
- `DbSet<{ResourceName}> {ResourceName}s` property following existing conventions

## Implementation Guidelines

### JSON Field Mapping
- Define specific field mappings from JSON response to C# model properties
- Document any special parsing requirements or transformations needed

### Parsing Considerations
- Use fail-fast parsing - let missing required fields throw immediately
- Apply individual property assignment pattern for all fields
- Handle empty or null arrays gracefully
- Skip invalid entries that don't meet minimum requirements

### ETL Processing Logic
- Ensure ETL is idempotent and can be run multiple times safely
- Only process items that don't already exist in the database
- Handle API rate limiting and errors gracefully through base class

### Success Criteria
- {ResourceName} domain model includes all required properties with correct data types
- {ResourceName}IndexEndpoint correctly parses all {resource} entries from JSON fixture
- {ResourceName}IndexETL successfully creates {ResourceName} records
- All unit tests pass
- Code follows established fail-fast and individual assignment patterns
- Database context includes {ResourceName} entity
- ETL is idempotent and can be run multiple times safely

### Out of Scope
- Do not create database migrations
- Do not modify existing ETL functionality  
- Do not implement {ResourceName} enrichment (that would be a separate {ResourceName}ETL plan)
- Do not parse complex nested objects unless specifically required
- Do not change existing JSON fixtures

## Template Usage Instructions

When using this template:

1. **Replace all placeholder variables**:
   - `{ResourceName}` - PascalCase name (e.g., "Pet", "Profession", "Recipe")
   - `{resource}` - lowercase name (e.g., "pet", "profession", "recipe")  
   - `{API_BASE_URL}` - Base API URL (e.g., "https://us.api.blizzard.com/data/wow")
   - `{table_name}` - Database table name (e.g., "pet", "profession", "recipe")

2. **Customize specific sections**:
   - Add resource-specific properties to the model structure
   - Define actual JSON field mappings
   - Specify any special parsing requirements
   - Add resource-specific success criteria

3. **Remove template instructions** (this section) from the final plan

4. **Verify completeness** by checking against existing similar plans for consistency