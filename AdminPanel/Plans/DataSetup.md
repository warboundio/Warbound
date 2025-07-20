# Plan: Data Setup

## Intent

Create an in-memory data mapping system for the Admin Panel that loads all collectible data from the database into dictionaries for fast access throughout the application. This will provide a centralized, high-performance data access layer for admin operations and enable quick overview of data collection status.

## Context

The AdminPanel currently lacks easy access to the rich collectible data stored in the BlizzardAPI database tables. The Data project already contains all necessary entities (ItemAppearances, Items, Toys, Mounts, Pets, Professions, ProfessionMedias, Realms, ItemMedias, Recipes, RecipeMedias) with proper Entity Framework configuration, but there's no mechanism for the AdminPanel to efficiently access this data.

The existing infrastructure provides:
- BlizzardAPIContext with DbSets for all collectible entities
- Established dependency injection patterns in AdminPanel/Program.cs
- Blazor component structure for creating new pages
- Entity models with standardized Id properties for dictionary key usage

## Scope

### In Scope

1. **WarcraftData Singleton Class**
   - Create Data/WarcraftData.cs as centralized data access point
   - Load all collectible data into Dictionary<int, T> collections by entity Id
   - Lazy initialization pattern - data loads on first access
   - Public properties exposing dictionaries for each entity type
   - Thread-safe singleton implementation

2. **Data Loading Logic**
   - Use BlizzardAPIContext to query all entities from database
   - Transform query results into dictionaries keyed by Id
   - Handle potential database connection issues gracefully
   - Log loading status and data counts via Core.Logs.Logging

3. **Dependency Injection Setup**
   - Register WarcraftData as singleton in AdminPanel/Program.cs
   - Ensure proper service lifetime management
   - Make WarcraftData available throughout Blazor components

4. **Collections Overview Page**
   - Create AdminPanel/Components/Pages/Collections/Overview.razor
   - Display count of entities in each data category
   - Clean, simple table or card layout showing data statistics
   - Use WarcraftData singleton to access count information

5. **Navigation Integration**
   - Add Collections/Overview to navigation structure
   - Ensure page is accessible from AdminPanel interface

### Out of Scope

- Real-time data synchronization with database changes
- Data caching expiration or refresh mechanisms
- Memory usage optimization for large datasets
- Data modification or update capabilities through WarcraftData
- Complex data relationships or joins beyond basic entity loading
- Performance monitoring or metrics collection
- Error recovery or retry logic for failed data loads

## Technical Approach

### WarcraftData Class Design

```csharp
// Data/WarcraftData.cs
public sealed class WarcraftData
{
    private static readonly Lazy<WarcraftData> _instance = new(() => new WarcraftData());
    public static WarcraftData Instance => _instance.Value;

    // Entity dictionaries - public readonly properties
    public Dictionary<int, ItemAppearance> ItemAppearances { get; private set; } = new();
    public Dictionary<int, Item> Items { get; private set; } = new();
    public Dictionary<int, Toy> Toys { get; private set; } = new();
    public Dictionary<int, Mount> Mounts { get; private set; } = new();
    public Dictionary<int, Pet> Pets { get; private set; } = new();
    public Dictionary<int, Profession> Professions { get; private set; } = new();
    public Dictionary<int, ProfessionMedia> ProfessionMedias { get; private set; } = new();
    public Dictionary<int, Realm> Realms { get; private set; } = new();
    public Dictionary<int, ItemMedia> ItemMedias { get; private set; } = new();
    public Dictionary<int, Recipe> Recipes { get; private set; } = new();
    public Dictionary<int, RecipeMedia> RecipeMedias { get; private set; } = new();

    // Initialization state tracking
    public bool IsLoaded { get; private set; }
    public DateTime? LoadedAt { get; private set; }
}
```

### Data Loading Strategy

1. **Initialization Method**:
   - `LoadDataAsync()` method called on first access
   - Use BlizzardAPIContext to query all entities
   - Convert each DbSet to Dictionary<int, T> using .ToDictionary(x => x.Id)
   - Set IsLoaded = true and LoadedAt = DateTime.UtcNow after successful load

2. **Error Handling**:
   - Catch database exceptions and log via Core.Logs.Logging.Error
   - Continue with empty dictionaries if data loading fails
   - Allow application to function with empty data rather than crash

3. **Thread Safety**:
   - Use Lazy<T> for singleton instantiation
   - Ensure LoadDataAsync is only called once
   - Lock access during initialization if needed

### Program.cs Integration

```csharp
// Add to AdminPanel/Program.cs service registration
builder.Services.AddSingleton<WarcraftData>();

// Initialize data on startup
var app = builder.Build();
var warcraftData = app.Services.GetRequiredService<WarcraftData>();
// Trigger initialization without awaiting to avoid blocking startup
_ = Task.Run(() => warcraftData.LoadDataAsync());
```

### Overview Page Implementation

1. **Page Structure**:
   - Standard Blazor page with @page "/collections/overview"
   - Inject WarcraftData singleton
   - Display entity counts in organized layout

2. **Display Format**:
   - Table or card-based layout showing entity names and counts
   - Categories: Items, ItemAppearances, Toys, Mounts, Pets, Professions, ProfessionMedias, Realms, ItemMedias, Recipes, RecipeMedias
   - Show "Loading..." state if data not yet loaded
   - Display LoadedAt timestamp when available

3. **Component Code**:
   ```csharp
   @page "/collections/overview"
   @inject WarcraftData WarcraftData

   <h3>Collections Overview</h3>
   @if (!WarcraftData.IsLoaded)
   {
       <p>Loading data...</p>
   }
   else
   {
       <table class="table">
           <thead>
               <tr><th>Category</th><th>Count</th></tr>
           </thead>
           <tbody>
               <tr><td>Items</td><td>@WarcraftData.Items.Count</td></tr>
               <!-- Additional rows for each entity type -->
           </tbody>
       </table>
       <p>Data loaded at: @WarcraftData.LoadedAt</p>
   }
   ```

## Edge Cases & Testing Needs

### Edge Cases

- Database connection failures during data loading
- Empty database tables (new/test environment)
- Large datasets causing memory pressure
- Concurrent access during initialization

### Error Handling

- Graceful degradation with empty dictionaries if loading fails
- Logging of all errors and data loading statistics
- Non-blocking initialization to prevent application startup delays

### Testing Strategy

- Create unit tests for WarcraftData singleton behavior
- Test data loading logic with mock BlizzardAPIContext
- Verify dictionary population and count accuracy
- Test Overview page rendering with loaded and unloaded states

## Success Criteria

### Functional Requirements

- WarcraftData singleton successfully loads all entity data into dictionaries
- Overview page displays accurate counts for all data categories
- Data accessible throughout AdminPanel via dependency injection
- Page navigation works correctly for Collections/Overview route
- Application startup not blocked by data loading process

### Technical Requirements

- Single instance of WarcraftData throughout application lifetime
- Memory-efficient dictionary storage keyed by entity Id
- Thread-safe initialization and access patterns
- Proper error handling and logging for failure scenarios
- Clean separation between data loading and UI concerns

### Data Integrity

- Dictionary counts match actual database entity counts
- All entity types properly represented in dictionaries
- No data loss during database-to-dictionary transformation
- Consistent data state across all AdminPanel components using WarcraftData

## Implementation Files

### New Files
- `Data/WarcraftData.cs` - Main singleton data class
- `AdminPanel/Components/Pages/Collections/Overview.razor` - Data overview page

### Modified Files
- `AdminPanel/Program.cs` - Register WarcraftData singleton
- `AdminPanel/Components/Layout/NavMenu.razor` - Add Collections/Overview navigation (if needed)

### Referenced Context
- `Data/BlizzardAPI/BlizzardAPIContext.cs` - Database context for entity loading
- `Data/BlizzardAPI/Endpoints/*.cs` - All entity models for dictionary population
- `Core/Logs/Logging.cs` - Error and status logging functionality