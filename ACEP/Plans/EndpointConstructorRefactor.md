# ACEP Plan: Refactor Endpoint Object Construction Pattern

This plan will refactor all existing Blizzard API endpoints to follow the new fail-fast parsing pattern established in `PetIndexEndpoint.cs`, removing object initializer syntax and default values to ensure clear error identification during JSON parsing failures.

## 1. Read Current Documentation
- Review **ETL/BlizzardAPI/Endpoints/_Endpoints.Agent.md** to understand the updated parsing conventions:
  - Each property must be parsed on its own line/property setter
  - Do not use object initializer syntax (`new() { ... }`)
  - Do not provide default values (e.g., `?? string.Empty`)
  - Use non-null assertion (`!`) for required string properties
  - Let parsing fail immediately if a property is missing

## 2. Reference Implementation
- Study **ETL/BlizzardAPI/Endpoints/PetIndexEndpoint.cs** as the correct pattern:
  ```csharp
  Pet petObj = new();
  petObj.Id = pet.GetProperty("id").GetInt32();
  petObj.Name = pet.GetProperty("name").GetString()!;
  petObj.Status = ETLStateType.NEEDS_ENRICHED;
  petObj.LastUpdatedUtc = DateTime.UtcNow;
  ```

## 3. Endpoints Requiring Refactoring
Refactor the `Parse` method in all the following endpoints to follow the new pattern:

### Index Endpoints (return `List<T>`)
- **ToyIndexEndpoint.cs** - Lines 18-24: Replace object initializer with individual property assignments
- **MountIndexEndpoint.cs** - Lines 16-20: Replace object initializer with individual property assignments

### Individual Resource Endpoints (return single object)
- **ToyEndpoint.cs** - Lines 18-26: Replace object initializer with individual property assignments
- **MountEndpoint.cs** - Lines 29-37: Replace object initializer with individual property assignments  
- **ItemEndpoint.cs** - Lines 28-45: Replace object initializer with individual property assignments
- **RealmEndpoint.cs** - Review and refactor if using object initializers
- **ItemAppearanceEndpoint.cs** - Review and refactor if using object initializers
- **ItemAppearanceSlotEndpoint.cs** - Review and refactor if using object initializers
- **ItemMediaEndpoint.cs** - Review and refactor if using object initializers
- **RealmIndexEndpoint.cs** - Review and refactor if using object initializers

## 4. Refactoring Rules
For each endpoint's `Parse` method:

1. **Replace object initializers**: Change from `new T { Prop1 = value, Prop2 = value }` to:
   ```csharp
   T obj = new();
   obj.Prop1 = value;
   obj.Prop2 = value;
   ```

2. **Remove default values**: Change from `GetString() ?? string.Empty` to `GetString()!`

3. **Preserve existing logic**: Keep any conditional logic or complex parsing but apply it to individual property assignments

4. **Maintain variable names**: Use descriptive variable names like `toyObj`, `mountObj`, `itemObj` for clarity

## 5. Testing Requirements
- Run all existing endpoint tests to ensure refactoring doesn't break functionality
- All 8+ endpoint tests must continue to pass
- Verify that parsing still works correctly with existing JSON fixtures

## 7. Validation
- Build the solution to ensure no compilation errors
- Run `dotnet test` on the endpoint test files to confirm all tests pass
- Verify that the refactored code follows the fail-fast approach where each property assignment can be traced to a specific line if it fails

## Success Criteria
- All endpoint `Parse` methods use individual property assignments instead of object initializers
- No default values are used for string properties (use `!` instead of `?? string.Empty`)
- All existing tests continue to pass
- Code follows the established fail-fast pattern for easier debugging
- The pattern is consistent across all endpoint files

## Out of Scope
- Do not modify the JSON fixtures or test files
- Do not change the endpoint URL building logic
- Do not modify the domain models or database context
- Do not create new tests; only ensure existing tests continue to pass
