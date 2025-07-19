# Blizzard API Endpoints Guide

This guide explains the conventions, patterns, and ethos behind how we define, implement, and test Blizzard API endpoint files for our ETL pipeline. It focuses on **why** things are done a certain way, not just **how**.

## File Structure Overview

Each resource in the `ETL.BlizzardAPI.Endpoints` namespace consists of:
- **Endpoint class**: `<Resource>Endpoint.cs`
- **Domain model**: `<Resource>.cs`
- **JSON fixture**: `<Resource>.json`
- **Unit test**: `<Resource>EndpointTests.cs`

## 1. Index vs. Enrichment Endpoints

- **Index endpoints**  
  - Do not require an ID in the constructor.  
  - Parse only the core fields:
    - `Id` (int)  
    - `Status = NEEDS_ENRICHED`  
    - `LastUpdatedUtc`
- **Enrichment endpoints**  
  - Require the resource ID in the constructor.  
  - Parse all fields from the JSON fixture.  
  - Set `Status = COMPLETE` when done.

## 2. Parsing & Fail‑Fast

- Each property is parsed in its own line/property setter.  
- Do not use object initializer syntax (`new() { ... }`); assign each property individually.
- Do not provide default values (e.g., `?? string.Empty`); let parsing fail if a property is missing.
- Use non-null assertion (`!`) for required string properties.
- If a JSON property is missing, parsing throws immediately, making it clear which field failed.

## 3. Status Handling

- Default domain model has `Status = NEEDS_ENRICHED`.  
- Enrichment endpoints override `Status` to `COMPLETE` after parsing.
- Index endpoints set `Status` to `NEEDS_ENRICHED` after parsing.

## 4. Naming & Fixtures

- Endpoint name, fixture name, and test name all derive from the resource name.  
- JSON fixtures must exist before implementing the endpoint; they drive the tests.

## 5. Unit Testing

- Tests read the JSON fixture, call `Parse`, and assert that:
  - Core fields are set correctly.
  - `Status` matches index/enrichment expectations.

## 6. Style & Rationale
- Look at existing endpoint files for coding and style patterns.

## 7. Entity Framework Context
- The `BlizzardAPIContext` is used to manage the database context for these endpoints. Please add your new object following coding conventions inside.

## 8. Out of Scope
- Do not create a migration for this new object.

> **Why this matters**  
> By documenting the thought process and conventions, we ensure consistency, maintainability, and clear on‑boarding for Copilot agents and new developers.
