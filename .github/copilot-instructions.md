# The GitHub Issue Outline
The number one goal you should have is to follow the outline of the GitHub issue.
Completing the issue is all about following the directions and the order of operations.
Begin with step 1.

# Documentation and Context
It's nearly impossible to give you all the context required to make every right decision. So we have markdown files available to you to ensure you're working within the system properly. We have .md files for context at three levels: the file level (example Logging.cs and next to it Logging.Agent.md), the namespace level (example inside the Core/ETL namespae there is a ETL.md), and the project level (example Project/Project.md). Required reading is every Project/Project.md. Please check these so you know how everything fits and works together. These files are meant to give you the context and intention of the code, not to be used as documentation for how to use the code.

# Plans
Many GitHub issues will have you build the plan before you execute it. Please refer to Core/Plans/PlanTemplate.md to understand how to write a plan.
Do not add additional categories or sections to the plan. This is meant to be succinct and quick to read. 

# Execution
When you are ready to execute or implement a plan (or you were asked to execute without a plan), please refer to the Execution Rules below

# Code Constraints
## KISS - Keep It Simple, Stupid. The more direct the code and the simpler the logic, the better. Do not add complexity for the sake of complexity.
We can and will be refactoring this code, constantly. So please keep it simple and readable to validate assumptions and logic.

## Refactoring
When asked to refactor do not use aliases, ensure the classes and files have the same name, and clean up all old references.

## Entity Framework
Do not create your own migrations. There is a critical step that happens in this pipeline that requires developers to validate and test your logic. 

## Execution Rules
Follow the plan exactly, do not expand it's approved scope. 

## Coding Philosophy
Fail fast.We need to always assume the happy path. This is all about keeping the code simple and readable.
Do not engage in defensive programming. It's OK if exceptions are thrown. You should not try to stop them. Let it crash the application. We will fix it in another pull request.
If the philosophy of the code has changed via a issue you are working on, please update the code and the corresponding .md files to reflect the new philosophy.
Do not comment the code. Code should be self-documenting.

## Core Tools
Core.Logs.Logging and Core.Settings.ApplicationSettings are the only tools you should use for logging and settings.

## Class Design
Avoid static classes, instance-based design is preferred for better testability and flexibility. Static methods are acceptable for utility functions, but avoid classes with all static methods.

## Testing
Blazor pages should not be tested. LUA should not be tested.
We do not do Integration tests. We only do unit tests.
Unit tets should be functional, small, and non-trival assertions.
Do not create mocks to test functionality.

## Boy Scout Rule
Leave the code cleaner than you found it. If a file you're already working on does not match up with our philosophy, please correct it.

## Naming Conventions
Code should be self-documenting. Properties should be used to document the code, never comments.
> bool isInsideInitialDelay = now.Subtract(createdAt).TotalMinutes < INITIAL_DELAY_MINUTES; if (isInsideInitialDelay) { continue; }
This is a good example of self-documenting code. The variable name is descriptive and explains the logic without needing comments.

# One Final Reiteration
Follow the GitHub issue outline. This is the most important part of your job. If you do not follow the outline, you will not complete the issue correctly.