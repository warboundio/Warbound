# Plan: Implement JournalExpansionETL

## Intent  
Add Journal Expansion enrichment ETL to enhance stub records with detailed dungeon and raid information from individual expansion endpoints.

## Context  
Building on the existing JournalExpansionIndexETL that creates stub records, this enrichment ETL fetches detailed data from individual journal expansion endpoints to populate DungeonIds and RaidIds fields. This follows the established enrichment pattern used by MountETL and other similar ETLs that process NEEDS_ENRICHED records and set them to COMPLETE status.

## In Scope  
- Add DungeonIds and RaidIds string properties to JournalExpansion model with appropriate MaxLength attributes
- Create JournalExpansionEndpoint to parse individual expansion data from Blizzard API
- Implement JournalExpansionETL following RunnableBlizzardETL pattern to process NEEDS_ENRICHED records
- Extract semicolon-delimited lists from dungeons.id and raids.id arrays in API response
- Set ETL status to COMPLETE after successful enrichment

## Acceptance Criteria  
- Given a JournalExpansion record with Status NEEDS_ENRICHED, when JournalExpansionETL processes it, then DungeonIds and RaidIds fields are populated with semicolon-delimited ID lists
- Given the API response from journal-expansion endpoint, when JournalExpansionEndpoint parses dungeons array, then DungeonIds contains "227;228;63;230;1277;1276" format string
- Given the API response from journal-expansion endpoint, when JournalExpansionEndpoint parses raids array, then RaidIds contains "741;742;743;744;1301" format string  
- Given successful enrichment processing, when examining the updated record, then Status equals COMPLETE and LastUpdatedUtc reflects current time