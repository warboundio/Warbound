# DUCA Plan: Create Pet Index ETL
This plan will scaffold and implement a new ETL that retrieves the World of Warcraft Pet Index from Blizzard's Developer API.

## 1. Read Endpoint Instructions
- Open and follow **ETL/BlizzardAPI/Endpoints/_Endpoints.Agent.md** to understand how to define a new endpoint and its data model.
- Refer to **ETL/BlizzardAPI/Endpoints/PetsIndex.json** for an example of the API's return structure.

## 2. Read ETL Instructions
- Open and follow **ETL/ETLs/_ETLs.Agent.md** to understand how to scaffold a new ETL class and the required method signatures.

## 3. Implementation Details
- **URL** `https://us.api.blizzard.com/data/wow/pet/index?namespace=static-us&locale=en_US`
- C# Model Structure   
   - SPECIFIC: `Id` and the `Name` of each pet
   - GENERAL: Include `Status` and `LastUpdatedUtc` properties.
