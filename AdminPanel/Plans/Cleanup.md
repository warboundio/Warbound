# Plan: AdminPanel Cleanup

## Intent  
Clean up AdminPanel by removing Blazor template files, reorganizing admin components, and improving code structure to create a focused internal tool for validating Warbound core systems.

## Context  
The AdminPanel was built quickly using Blazor Server templates and now contains unnecessary template components (Counter, Weather) that don't serve the admin validation purpose. The WorkflowThreadView component needs proper organization in the Admin section, and inline CSS should be extracted to follow proper separation of concerns.

## In Scope  
- Remove Counter and Weather template pages and their navigation menu entries
- Move WorkflowThreadView component from root Components to Components/Pages/Admin
- Change application default route from Home ("/") to DUCA admin page ("/admin/duca")
- Extract inline CSS from Duca.razor component to separate CSS file
- Remove completed "Cleanup" draft from Drafts.md

## Acceptance Criteria  
- Given the application starts, when no specific route is provided, then it redirects to /admin/duca
- Given a user views the navigation menu, when examining available options, then Counter and Weather links are not present
- Given WorkflowThreadView is accessed within Duca page, when the component loads, then it functions properly from its new Admin folder location
- Given the Duca admin page loads, when examining the component source, then styles are applied from external CSS file not inline style blocks
- Given Drafts.md is reviewed, when looking for cleanup items, then the completed cleanup draft is removed