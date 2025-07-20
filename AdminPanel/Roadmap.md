# AdminPanel Roadmap

## Collection Visualizer

A comprehensive display system for all known collectible items that provides visual feedback on collection status and organizational capabilities.

**Key Features:**
- Marks owned vs unowned items visually (e.g., greyed out vs full color)
- Organizes items into multiple view modes (by category, by expansion, by type)
- Allows drilling into specific items for deeper analysis and detailed information
- Provides filtering and search capabilities across the entire collection database

This feature serves as the primary interface for understanding collection completeness and identifying collection gaps.

---

## Metadata Editor

An interactive editing system that allows manual enhancement and override of Blizzard-provided item data with contextual information critical to the Warbound experience.

**Key Features:**
- Click-to-edit interface for any collectible item
- Override capability for Blizzard-provided fields when incorrect or incomplete
- Custom metadata fields including:
  - Item obtain methods and requirements
  - Geographic context (map/zone/NPC associations)
  - Faction/reputation prerequisites
  - Seasonal or limited-time availability
- All edits stored in supplemental tables layered on top of core Blizzard data
- Change tracking and revision history for data integrity

This system enables the enrichment that transforms raw API data into actionable collection guidance.

---

## Source Mapping System

A comprehensive tool for exploring, documenting, and editing the various ways collectible items can be obtained in World of Warcraft.

**Key Features:**
- Source type classification (NPC, Quest, Drop, Vendor, Achievement, etc.)
- Source ID tracking and validation
- Map ID and coordinate specification for location-based sources
- Drop behavior documentation (BOP/BOE, daily lockouts, shared loot pools)
- Probability and farming efficiency data
- Alternative source tracking for items with multiple obtain methods

This layer is critical to enriching the Warbound system and will eventually power intelligent guides and optimized farming routes.

---

## Guide Authoring Platform

A text-based content creation system for developing collection guides and strategies tied to specific items or collection goals.

**Key Features:**
- Rich text editor with simple formatting options
- Direct item and goal linking within guide content
- Template system for common guide structures
- Preview and validation tools for guide accuracy
- Version control for guide updates and improvements

Designed to test content structure and inspire future UI/UX planning for the main Warbound experience.

---

## Goal Infrastructure System

A flexible framework for defining, tracking, and managing collection objectives that can range from single items to comprehensive collection categories.

**Key Features:**
- Goal creation and definition interface
- User goal activation and prioritization
- Progress tracking and completion indicators
- Goal examples include:
  - Single item or mount acquisition
  - Complete transmog sets from specific expansions
  - Weapon collections by type and expansion
  - Achievement-based collection challenges
- Goal influence on data display priority and organization
- Scoring and progression algorithms for goal completion

This system lays the foundation for the scoring, progression, and endgame structure that will drive user engagement in Warbound.

---

## Reporting & Analytics Dashboard

A comprehensive evaluation system that provides insights into data completeness, guide coverage, and system health across the entire Warbound ecosystem.

**Key Features:**
- Data completeness metrics (percentage of items with known sources)
- Category-based completion analysis (most complete vs most lacking areas)
- Guide coverage reporting and gap identification
- ETL performance and data freshness monitoring
- User collection pattern analysis and trends

This serves as the developer dashboard providing a living snapshot of Warbound's data health, coverage completeness, and system performance.
