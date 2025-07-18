# Core.ETL Namespace Context

This namespace exists to unify scheduling, state, and execution of ETL jobs in a simple, agnostic way.

## Why We Built Core.ETL

- **Simplicity & Flexibility:**  
  We avoided custom interfaces or attributes on job classes. Instead, jobs are referenced by their fully qualified static method name and invoked via reflection.  
- **Centralized State:**  
  All job metadata (last run, success/failure, duration) and distributed locks live in a shared Postgres table. This lets multiple instances coordinate without extra infrastructure.  
- **Decoupled Scheduler:**  
  The runner loop and lock logic are general purpose. Any new job just needs a cron string entry in the DB and a static `RunAsync` method—no code changes to the scheduler.

## Files in This Namespace

- **ETLContext.cs**  
  Holds EF Core context for the `etl_jobs` table. Enables reading/updating job state and lock fields.  
- **ETLJob.cs**  
  Defines the schema for job records. Contains:
  - `Name`: fully qualified method for reflection.
  - `CRONSchedule`: when to run.
  - Lock fields: owner, timestamp, timeout.
  - History fields: last success, duration, status.
- **ETLRunner.cs**  
  Implements:
  - A continuous loop polling the DB.
  - Cron-based eligibility checks.
  - Distributed lock acquisition and release.
  - Reflection-based invocation of static `RunAsync` methods.
  - Automatic state updates on success or failure.
  - Manual trigger support for external commands.
