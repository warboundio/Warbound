# Warbound.io Addon

## Overview

The Warbound addon is a modular, event-driven Lua addon designed to collect and serialize meaningful **collection** and **gameplay** data from World of Warcraft. This data is stored locally in SavedVariables and offloaded to the Warbound client for analysis, display, and goal tracking on the web.

The addon focuses on two distinct areas:

1. **User Data (Collections)** – Tracks what the player has collected.
2. **Game Data (Background Metadata)** – Captures context about where things come from in the world.

These systems power the core value of Warbound.io: understanding and guiding the collecting journey in WoW.

---

## Feature Categories

### 1. 📦 Collections (User Data)

This is the backbone of the addon. It monitors the player's collection state in real time and builds a minimal snapshot of their unlocks.

**Scope:**
- Transmogs
- Pets
- Mounts
- Recipes
- Toys

**Key Concepts:**
- Each collection category is encoded into a compact string (e.g., using Base90) representing all unlocked IDs.
- The encoded strings are stored in `SavedVariables` under a unified table.
- On key game events (e.g. learning an appearance, recipe, or mount), the addon rebuilds the relevant string snapshot.
- Updates are low-cost and can be optimized later. Simplicity comes first.

**SavedVariables Example:**
```lua
WarboundData = {
  account_id = "<account-identifier>",
  seed       = <random-seed-int>,

  transmogs = "<encoded>",
  pets      = "<encoded>",
  mounts    = "<encoded>",
  recipes   = "<encoded>",
  toys      = "<encoded>",
}
```

---

### 2. 🧠 Background Metadata (Game Data)

This system captures *contextual world data* — what items are available where, how they're obtained, and what conditions apply.

**Use Cases:**
- Vendor item lists (including costs, reputation requirements)
- Quest and encounter sources
- World drops and dungeon trash
- Zone and map context
- NPC info (ID, location, etc.)

**Behavior:**
- Listens to vendor interactions, loot events, and other relevant sources.
- Skips data capture when the player is at a mailbox (to avoid false item source attribution).
- Stores data in memory for the Warbound client to consume.
- Future-proofed to track rep/faction requirements where possible.

This module is about empowering Warbound.io to guide players accurately through the game world without relying on external scraped sources.

---

## Logging & UX (Optional)

While not a formal third module, the addon will support a clean logging system to:

- Report collection gains (e.g., “+3 transmogs!”) at the end of dungeons or farming sessions.
- Use a dedicated, uncluttered chat channel (`/wb log`) for these summaries.
- Enable opt-in features like instance tracking or goal completion celebrations.

The intention is not to build a heavy in-game interface — Warbound's experience is web-first. But the in-game logging will provide clarity and fun feedback loops where they make sense.

---

## Philosophy

- **Modular & Minimal** – Each system is load-on-demand and can be toggled independently.
- **Efficient by Default** – Storage is compact, updates are lightweight, and client syncs are batched.
- **Trustworthy Data** – All collection and source data is observed firsthand — never scraped.
- **Opt-In Guidance** – Any UI or feedback in-game will be intentionally minimal, focused, and user-controlled.
