# Warbound Console Runner

## Overview

The Warbound console runner is a utility application that serves as the operational hub for the Warbound ecosystem. It provides a simple, centralized way to execute various background services, run one-off tasks, and coordinate the different components of the Warbound system.

---

## Current Implementation

The console runner currently includes:
- ETL loop runner for continuous data processing
- Discord bot service management
- Individual ETL job execution (e.g., RecipeETL)
- Logging configuration and management
- Centralized service coordination

---

## Core Functionality

### Service Orchestration

The primary function is to manage and coordinate the various services that make up the Warbound ecosystem:
- **ETL Runner**: Manages continuous data collection and processing loops
- **Discord Bot**: Handles community interaction and notification services
- **Individual Jobs**: Executes specific data processing tasks on demand

### Development Utilities

Provides a clean environment for running development and validation tasks:
- Quick execution of one-off code snippets
- Debug folder copying and deployment
- System validation and health checks
- Development workflow support tools

---

## Philosophy

- **Utility Knife Approach** – Quick and easy execution of various system components
- **Development Friendly** – Clean environment for testing and validation without test pollution
- **Operational Simplicity** – Single entry point for managing system services
- **Flexible by Design** – Easy to add new utilities and services as needed

The console runner may evolve or be replaced as the system grows, but it serves as an essential coordination point for the current Warbound infrastructure. It prioritizes practical utility over architectural elegance, making it easy to spin up services, test functionality, and manage the overall system health.