# Warbound.io Addon

## Overview

A single, modular Lua addon that provides three distinct features:

1. **Collections (Core)** – Serialize in‑game collection data (transmogs, pets, mounts, recipes, toys).
2. **Background Data (Data)** – Capture contextual metadata (quest/item sources, vendor IDs, etc.) for each collectible.
3. **Guidance (Guide)** – In‑game UI to walk players through active goals, with coordinates and copy‑to‑clipboard links tied to the website.

Modules load only when needed, so users install once and enable or disable features via a simple config.

---

## Modules

### 1. Core Module: Collections

* Hooks into collection events (`ADDON_LOADED`, `PLAYER_LOGOUT`, update events) to rebuild full snapshots of each category.
* Gathers collected item IDs for:
  * Transmogs
  * Pets
  * Mounts
  * Recipes
  * Toys
* Uses Base90 encoding to compress each ID list into a minimal-length string.

### 2. Data Module: Background Metadata

* Listens to quest completions, vendor interactions, drop events, and other hooks.
* Maps each collected ID to its source (quest ID, NPC vendor, drop table, etc.).
* Emits lightweight events or stores mappings in a transient table for the client to consume.

### 3. Guide Module: Goal Assistance

* Reads the Core snapshot and active goal definitions (provided by the website).
* Displays a step‑by‑step guide in‑game:

  * Target coordinates
  * Recommended zones
  * Copy‑to‑clipboard commands (e.g. `/wb copy MOUNT_HERALDOFGLOW`)
* Load‑on‑demand via slash commands or config toggles.

---

## SavedVariables Structure

All persistent data remains in a single SavedVariables table, with only the Core module writing to it:

```lua
WarboundData = {
  account_id = "<account identifier>",
  seed       = <random-seed-int>,

  -- Encoded snapshots for each collection category:
  transmogs = "<base90-string>",
  pets      = "<base90-string>",
  mounts    = "<base90-string>",
  recipes   = "<base90-string>",
  toys      = "<base90-string>",
}
```

## Encoding Details

* Base90 encoding is tuned solely for minimal string length.
* Each encoded string fully represents its category’s state.

---

## Load‑On‑Demand & Configuration

* `.toc` entries load modules only when their slash commands or events fire.
* A simple in‑addon config lets users enable/disable the Data or Guide modules.

---

## Installation & Usage

1. Copy the `Warbound` folder to `Interface/AddOns`.
2. Launch WoW; the addon initializes Core by default.
3. Use `/wb core` to view collection status, `/wb data` to inspect metadata, and `/wb guide <goal>` for on‑screen guidance.

---

## Extensibility

* Add new collection categories by updating Core’s encoder and adding a field.
* Plug additional metadata sources into the Data module via event hooks.
* Extend Guide with new UI panels or hotkeys.
