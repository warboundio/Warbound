# Warbound Admin Panel – Vision & Direction

## Purpose

This Blazor application is an **internal tool** designed to validate the core systems behind Warbound.io. It is not a production interface. It exists to help the developer (you) observe, test, and refine the data structures, collection logic, and gameplay metadata foundational to the broader Warbound project.

---

## Guiding Principles

- No authentication, no access control – this is for one developer.
- No focus on UX polish – only functionality that helps you validate assumptions.
- Clarity and feedback above all – it should show you what’s working, what’s missing, and what needs thought.
- Every change in this app might affect the addon, the ETL, or the site.

---

## Functional Overview

### 1. Collection Visualizer

Displays all known collectible items:
- Marks owned vs unowned items visually (e.g., greyed out vs full color)
- Organizes items into views (e.g., by category, by expansion, by type)
- Allows drilling into specific items for deeper analysis

### 2. Metadata Editor

Each item can be clicked into for manual edits:
- Allows override of Blizzard-provided fields
- Adds new metadata such as:
  - How the item is obtained
  - What map/zone/NPC it's tied to
  - Faction/reputation requirements
- All edits are stored locally in a supplemental table layered on top of core Blizzard data

### 3. Source Mapping

Allows exploration and/or editing of how items are obtained:
- Source types (e.g., NPC, Quest, Drop, Vendor)
- Source ID
- Map ID and coordinates
- Expected drop behavior (e.g., BOP, daily lockout, shared loot pool)

This layer is critical to enriching the Warbound system and will eventually power guides and farming routes.

### 4. Guide Authoring

Supports text-based entry of guidance content:
- Tied to specific items or goals
- Simple formatting only
- Designed to test structure and inspire future UI/UX planning

### 5. Goal Infrastructure (Preview)

Items can belong to one or more predefined goals:
- User can mark any goal as "active"
- Examples of goals:
  - A single item or mount
  - All transmog from Burning Crusade
  - All 2H axes from Wrath
- Goals influence how data is displayed and prioritized

This section lays the foundation for the scoring, progression, and endgame structure of Warbound.

---

## Reporting & Evaluation Tools

This app should help answer high-level questions like:
- What percentage of items have at least one known source?
- Which categories are most complete vs most lacking?
- What’s the state of guide coverage?

It serves as the developer dashboard – a living snapshot of Warbound's data health and coverage.

---

## Philosophy

- Features emerge as clarity improves – nothing is final or sacred
- Implementation choices are deferred – focus on *what* matters, not *how* it’s done
- Simplicity rules – everything should support clarity, iteration, and validation
