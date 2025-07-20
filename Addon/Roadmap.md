# Addon Roadmap

## World of Warcraft Lua Addon System

A modular, event-driven Lua addon designed to collect and serialize meaningful **collection** and **gameplay** data from World of Warcraft. This data would be stored locally in SavedVariables and offloaded to the Warbound client for analysis, display, and goal tracking on the web.

**Core Focus Areas:**
1. **User Data (Collections)** – Tracks what the player has collected
2. **Game Data (Background Metadata)** – Captures context about where things come from in the world

These systems would power the core value of Warbound.io: understanding and guiding the collecting journey in WoW.

---

## Collections Data System (User Data)

The backbone of the addon that monitors the player's collection state in real time and builds a minimal snapshot of their unlocks.

**Collection Scope:**
- Transmogs (appearances)
- Pets (battle pets and companions)
- Mounts (ground and flying mounts)
- Recipes (crafting patterns and plans)
- Toys (fun items and novelties)

**Key Implementation Features:**
- Each collection category encoded into compact strings (e.g., using Base90) representing all unlocked IDs
- Encoded strings stored in `SavedVariables` under a unified table structure
- Real-time updates on key game events (learning appearances, recipes, mounts)
- Low-cost updates optimized for performance
- Batch synchronization with Warbound client

**SavedVariables Structure:**
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

## Background Metadata System (Game Data)

System for capturing *contextual world data* about item sources, availability, and obtain conditions.

**Data Collection Targets:**
- Vendor item lists (including costs, reputation requirements)
- Quest reward and completion sources
- Encounter and boss drop sources
- World drop and dungeon trash sources
- Zone and map context information
- NPC information (ID, location, faction)

**Collection Behavior:**
- Event-driven listening to vendor interactions and loot events
- Smart filtering to avoid false attribution (mailbox interactions, etc.)
- In-memory storage for efficient client consumption
- Future-ready for reputation and faction requirement tracking

This module empowers Warbound.io to guide players accurately through the game world without relying on external scraped data sources.

---

## Logging & User Experience System

Clean, optional logging system providing meaningful feedback to players during their collection activities.

**Logging Features:**
- Collection gain summaries (e.g., "+3 transmogs!") after dungeons or farming sessions
- Dedicated, uncluttered chat channel (`/wb log`) for addon communications
- Opt-in features for instance tracking and goal completion celebrations
- Minimal in-game interface (web-first experience philosophy)

**User Experience Principles:**
- Intentional minimalism for in-game interactions
- Clear, actionable feedback loops where they add value
- Non-intrusive integration with existing gameplay flows

---

## LUA Subfolder Publishing System

C# utility system for streamlining the development and deployment workflow of the World of Warcraft addon.

**Publishing Features:**
- Automated copying of Lua addon files from development subfolder
- Direct publishing to World of Warcraft addons directory
- Development workflow optimization for testing and iteration
- Minimal configuration required for setup and use

**Development Workflow:**
- Edit Lua files in development environment
- Single command publishes to WoW addons folder
- Immediate testing capability in-game
- Version control friendly file organization

This utility serves as the bridge between development environment and World of Warcraft addon deployment, reducing manual file management overhead.
