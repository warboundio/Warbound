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

### Out of Scope

- Real-time data synchronization with database changes
- Data caching expiration or refresh mechanisms
- Memory usage optimization for large datasets
- Data modification or update capabilities through WarcraftData
- Complex data relationships or joins beyond basic entity loading
- Performance monitoring or metrics collection
- Error recovery or retry logic for failed data loads
