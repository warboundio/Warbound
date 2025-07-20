# Admin Panel Drafts

## Draft: Implement Collection Visualizer (Overall)
### Agent
Create a display system for all known collectible items that provides visual feedback on collection status and organizational capabilities. This includes marking owned versus unowned items visually, organizing items into view modes by category, expansion, or type, allowing drill-down into specific items for more information, and providing filtering and search across the entire collection database. This serves as the primary UI for understanding collection completeness and identifying gaps.

## Draft: Implement Toy Collection Visualizer
### Agent
Create a display system for all known collectible toys that provides visual feedback on collection status and organizational capabilities. This includes marking owned versus unowned toys visually, organizing toys into view modes by category, expansion, or type, allowing drill-down into specific toys for more information, and providing filtering and search across the entire toy collection database. This serves as the primary UI for understanding toy collection completeness and identifying gaps.

## Draft: Implement Pet Collection Visualizer
### Agent
Create a display system for all known collectible pets that provides visual feedback on collection status and organizational capabilities. This includes marking owned versus unowned pets visually, organizing pets into view modes by category, expansion, or type, allowing drill-down into specific pets for more information, and providing filtering and search across the entire pet collection database. This serves as the primary UI for understanding pet collection completeness and identifying gaps.

## Draft: Implement Mount Collection Visualizer
### Agent
Create a display system for all known collectible mounts that provides visual feedback on collection status and organizational capabilities. This includes marking owned versus unowned mounts visually, organizing mounts into view modes by category, expansion, or type, allowing drill-down into specific mounts for more information, and providing filtering and search across the entire mount collection database. This serves as the primary UI for understanding mount collection completeness and identifying gaps.

## Draft: Implement Transmog Collection Visualizer
### Agent
Create a display system for all known collectible transmogs that provides visual feedback on collection status and organizational capabilities. This includes marking owned versus unowned transmogs visually, organizing transmogs into view modes by category, expansion, or type, allowing drill-down into specific transmogs for more information, and providing filtering and search across the entire transmog collection database. This serves as the primary UI for understanding transmog collection completeness and identifying gaps.

## Draft: Implement Recipe Collection Visualizer
### Agent
Create a display system for all known collectible recipes that provides visual feedback on collection status and organizational capabilities. This includes marking owned versus unowned recipes visually, organizing recipes into view modes by category, expansion, or type, allowing drill-down into specific recipes for more information, and providing filtering and search across the entire recipe collection database. This serves as the primary UI for understanding recipe collection completeness and identifying gaps.

## Draft: Implement Metadata Editor
### Agent
Build an interactive editing system that allows manual enhancement and override of Blizzard-provided item data. The editor should allow click-to-edit capabilities on any collectible item and support custom metadata fields including obtain methods, geographic context, faction requirements, and seasonal availability. Overrides should be stored in supplemental tables layered on top of Blizzard data with full change tracking and revision history.

## Draft: Build Source Mapping System
### Agent
Develop a toolset for documenting and exploring how collectible items are obtained. This includes classification of source types such as NPC, Quest, Drop, Vendor, and Achievement, tracking and validation of source IDs, specification of map IDs and coordinates for location-based sources, documentation of drop behaviors including lockouts and loot pool types, and capturing probability or efficiency data. This system is critical for powering future intelligent guide systems.

## Draft: Create Guide Authoring Platform
### Agent
Create a basic authoring tool for writing text-based collection guides. It should support rich text editing with formatting, linking items and goals within content, templating for common guide types, previewing and validating guide output, and version control for managing guide updates over time. This tool will help define future UX for guides and support internal content development.

## Draft: Implement Goal Infrastructure System
### Agent
Build a framework for defining and managing collection goals. Goals should be user-activatable and support tracking progress toward completion. Example goals include single item acquisitions, full transmog sets from a specific expansion, weapon type collections, and achievement-based goals. Goals will influence how data is displayed and scored, and this system lays the foundation for long-term progression.

## Draft: Build Reporting and Analytics Dashboard
### Agent
Implement a reporting system that evaluates data coverage and health across the Warbound ecosystem. It should report on data completeness for known sources, identify most and least complete item categories, highlight guide coverage gaps, monitor ETL performance and data freshness, and analyze user collection trends. This dashboard will serve as the developer's primary insight into system integrity.