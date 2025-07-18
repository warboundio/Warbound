# Blizzard ETL Jobs Guide

This guide explains the conventions, patterns, and ethos behind creating runnable ETL jobs for Blizzard API data in the `ETL.ETLs` namespace. It highlights **why** we structure ETLs a certain way, and **how** to implement new ones consistently.

## 1. Entry Point Signature

Every ETL class **must** include the exact `RunAsync` signature:
```csharp
public static async Task RunAsync(ETLJob? job = null) 
    => await RunAsync<YourETLClass>(job);
```
- Enables the job to be discovered, scheduled, and run on demand.
- **Do not modify** this signature outside of the class type (YourETLClass).

## 2. Required Overrides

ETL classes inherit from `RunnableBlizzardETL` and **must** implement:
```csharp
protected override async Task<List<object>> GetItemsToProcessAsync();
protected override async Task UpdateItemAsync(object item);
```

### GetItemsToProcessAsync
- Returns a list of items (IDs or domain objects) that need processing.
- Example (Index ETL):  
  - Query existing table for missing IDs.  
  - Return IDs to insert.

### UpdateItemAsync
- Called for each item returned above.
- Example (Enrichment ETL):  
  - Cast the `object` back to the domain model.  
  - Call the corresponding endpoint (`endpoint.GetAsync()`).  
  - Update individual properties on the model.  
  - Set `Status` (`NEEDS_ENRICHED` → `COMPLETE` for enrichment).  
  - Update `LastUpdatedUtc`, then add to `SaveBuffer`.

## 3. Index vs. Enrichment ETLs
- **Index**: seeds the database with minimal stub records.
- **Enrichment**: fills in details on existing stubs.

## 4. SaveBuffer & Async Patterns
- Use `SaveBuffer.Add(...)` to queue updates/inserts.
- `UpdateItemAsync` may include `await Task.Run` to satisfy async signature.

## 5. Examples
- **ItemAppearanceIndexETL**  
  - Enumerates `SlotURLTypes`, fetches IDs, filters out existing, returns new IDs.  
  - `UpdateItemAsync`: creates new `ItemAppearance` stubs.

- **ItemAppearanceETL**  
  - Loads `ItemAppearances` needing enrichment.  
  - Calls `ItemAppearanceEndpoint` per ID.  
  - Updates fields: `SlotType`, `ClassType`, etc., sets `Status = COMPLETE`.

Refer to these existing classes for patterns and implementation details.

## 6. Special Case: Validation ETLs

**Validation ETLs** are a special category that do **not** inherit from `RunnableBlizzardETL`:
- Located in `ETL.ETLs.Validation` namespace
- Still require the same `RunAsync` entry point signature
- Focus on schema validation rather than data processing
- Use `BlizzardAPIRouter.GetJsonRawAsync(url, forceLiveCall: true)` to bypass cache
- Example: `SchemaValidationETL` compares JSON fixtures against live API responses

These ETLs handle monitoring and validation tasks that don't fit the standard data processing pattern.

## 7. Style & Rationale

- **Minimal Boilerplate**: base class handles retries, logging, error handling.
- **Clarity**: split discovery vs. enrichment keeps ETLs focused.
- **Consistency**: same method names and signatures across all ETLs.
- **Maintainability**: by codifying these patterns, new ETLs onboard quickly.

> **Why this matters**  
> A clear ETL structure ensures robust, reproducible data workflows and makes it easy for Copilot agents (and developers) to add or update jobs without guesswork.
