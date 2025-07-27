# ItemExpansion ETL Registration

The ItemExpansion ETL has been implemented and is ready for registration in the ETL system.

## ETL Job Registration

To register the ItemExpansionETL in the system, add a record to the `etl_jobs` table in the `application` schema:

```sql
INSERT INTO application.etl_jobs (id, name, cronschedule, locktimeoutminutes, waslastrunsuccessful)
VALUES (
    gen_random_uuid(),
    'Data.ETLs.ItemExpansionETL.RunAsync',
    '0 0 6 * * *',  -- Run daily at 6 AM
    60,
    true
);
```

## ETL Functionality

The ItemExpansionETL processes all items in the `item` table and maps them to their corresponding World of Warcraft expansions by:

1. Analyzing semicolon-delimited item lists in `JournalEncounter.Items`
2. Finding the instance ID associated with encounters containing each item
3. Matching instance IDs to expansion IDs via `JournalExpansion.DungeonIds` and `JournalExpansion.RaidIds`
4. Creating `ItemExpansion` records with the discovered mappings
5. Setting `ExpansionId` to `-1` for items that cannot be mapped to any expansion

## Schedule Recommendation

The ETL should be scheduled to run after:
- `JournalEncounterETL` (to ensure encounter data is enriched)
- `JournalExpansionETL` (to ensure expansion data is available)
- `ItemETL` (to ensure item data is current)

A daily schedule is appropriate since this mapping data doesn't change frequently.

## Manual Execution

The ETL can be triggered manually via Discord command or direct method invocation:

```csharp
await Data.ETLs.ItemExpansionETL.RunAsync();
```