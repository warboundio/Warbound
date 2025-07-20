# Warbound Admin Panel

## Purpose

This Blazor application is an **internal tool** designed to validate the core systems behind Warbound.io. It is not a production interface. It exists to help the developer (you) observe, test, and refine the data structures, collection logic, and gameplay metadata foundational to the broader Warbound project.

---

## Guiding Principles

- No authentication, no access control � this is for one developer.
- No focus on UX polish � only functionality that helps you validate assumptions.
- Clarity and feedback above all � it should show you what�s working, what�s missing, and what needs thought.
- Every change in this app might affect the addon, the ETL, or the site.

---

## Current Implementation

The AdminPanel currently consists of a basic Blazor Server application with:
- Standard Blazor template structure
- Interactive server components enabled
- Basic routing and error handling
- Default template pages (Home, Counter, Weather)

---

## Philosophy

- Features emerge as clarity improves � nothing is final or sacred
- Implementation choices are deferred � focus on *what* matters, not *how* it�s done
- Simplicity rules � everything should support clarity, iteration, and validation
