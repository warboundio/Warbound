# ACEP Plan: Create Blizzard API Schema Validation ETL

This plan will create an automated schema validation ETL that monitors Blizzard API endpoints for schema changes, replacing the need for manual integration tests. The system will run on a scheduled basis to ensure our JSON fixtures remain synchronized with live API responses.

## 1. Read ETL Instructions
- Open and follow **ETL/ETLs/_ETLs.Agent.md** to understand how to create a validation ETL class that processes schema comparisons.
- Review **RunnableBlizzardETL.cs** as the base class for implementation patterns.

## 2. Read Endpoint Instructions  
- Open and follow **ETL/BlizzardAPI/Endpoints/_Endpoints.Agent.md** to understand endpoint patterns and JSON fixture usage.
- Study **ETL/BlizzardAPI/General/BlizzardAPIRouter.cs** to understand the `GetJsonRawAsync(url, forceLiveCall: true)` method for live API calls.

## 3. Create Validation Folder Structure
Create **ETL/ETLs/Validation/** subfolder to organize schema validation ETLs:
- This separates validation logic from data processing ETLs
- Follows established folder organization patterns
- Enables future expansion of validation-related ETLs

## 4. Create Schema Validation ETL
Create **ETL/ETLs/Validation/SchemaValidationETL.cs** following these requirements:
- Inherit from `RunnableBlizzardETL`
- Include required `RunAsync` entry point signature
- `GetItemsToProcessAsync`: Return list of endpoint configurations (JSON file path, ID, endpoint URL pattern)
- `UpdateItemAsync`: For each endpoint, compare local JSON fixture against live API response schema
- Use `BlizzardAPIRouter.GetJsonRawAsync(url, forceLiveCall: true)` to bypass cache
- Log schema differences with appropriate severity levels
- Handle API failures gracefully (rate limits, timeouts, etc.)

## 5. Schema Comparison Logic
Implement schema validation that:
- Extracts JSON schema structure (property names, types, nested object patterns)
- Compares local fixture schema against live API response schema
- Identifies missing properties, type changes, or structural differences
- Reports findings through structured logging for monitoring systems
- Does not require exact value matches - focuses on schema structure only

## Implementation Details

### Endpoint Configurations
Target the following JSON fixtures and their corresponding API endpoints:
- **Item.json** (ID: 19019) → `https://us.api.blizzard.com/data/wow/item/19019?namespace=static-us&locale=en_US`
- **Mount.json** (ID: 35) → `https://us.api.blizzard.com/data/wow/mount/35?namespace=static-us&locale=en_US`  
- **Pet.json** (ID: 39) → `https://us.api.blizzard.com/data/wow/pet/39?namespace=static-us&locale=en_US`
- **Toy.json** (ID: 1131) → `https://us.api.blizzard.com/data/wow/toy/1131?namespace=static-us&locale=en_US`

### Schema Extraction Approach
- Parse JSON into `JsonElement` structures
- Recursively traverse property hierarchy
- Build schema fingerprint based on property names and types
- Handle optional properties and nullable fields appropriately
- Focus on structural validation, not data validation

### Logging Strategy
- **INFO**: Successful schema validation (schemas match)
- **WARNING**: Minor schema differences (new optional properties)
- **ERROR**: Major schema breaking changes (missing required properties, type changes)
- Include endpoint URL, fixture file name, and specific differences in log messages

### Error Handling
- Graceful handling of API rate limits and temporary failures
- Retry logic for transient network issues
- Fallback behavior when live API is unavailable
- Clear distinction between API failures vs. schema differences

## Success Criteria
- SchemaValidationETL successfully compares all four endpoint schemas
- Live API calls bypass cached responses using `forceLiveCall: true`
- Schema differences are logged with appropriate detail and severity
- ETL follows established `RunnableBlizzardETL` patterns
- Code integrates cleanly with existing ETL scheduling infrastructure
- Validation runs without affecting existing data processing ETLs

## Out of Scope
- Do not modify existing JSON fixtures or endpoint implementations
- Do not create database tables or entities for validation results
- Do not implement real-time schema monitoring or alerting systems
- Do not modify existing ETL scheduling or job management systems
- Do not create user interfaces or dashboards for validation results