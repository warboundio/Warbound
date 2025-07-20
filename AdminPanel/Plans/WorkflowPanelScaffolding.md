# DUCA Plan: Implement Workflow Panel Scaffolding

This plan implements the initial Blazor admin panel page for managing DUCA workflows, creating a foundation for workflow creation and management within the AdminPanel project.

## 1. Read AdminPanel Context

- Open and follow **AdminPanel/AdminPanel.md** to understand the project's purpose as an internal validation tool and its guiding principles.
- Review the existing Blazor Server application structure in **AdminPanel/Components** to understand current navigation and layout patterns.
- Examine **AdminPanel/Components/Layout/NavMenu.razor** to understand how navigation items are structured and styled.

## 2. Create Admin/Duca.razor Component

Create **AdminPanel/Components/Pages/Admin/Duca.razor** following these requirements:
- Use `@page "/admin/duca"` directive for routing
- Include `@rendermode InteractiveServer` for interactive functionality
- Follow existing Blazor component patterns from **AdminPanel/Components/Pages**
- Include appropriate `<PageTitle>` with "DUCA Workflow Panel"
- Use semantic HTML structure with clear sections

## 3. Implement Project Selection Dropdown

Create a dropdown component for major project selection:
- Include all three major projects: "Addon", "AdminPanel", "Data"
- Use HTML `<select>` element with proper `id` and `name` attributes
- Add readable label: "Target Project"
- Include default option: "Select a project..."
- Apply consistent styling with existing AdminPanel components
- Bind to a component property for state management

## 4. Implement Title Input Field

Create a title input component:
- Use HTML `<input type="text">` element
- Add readable label: "Workflow Title"
- Include helpful placeholder: "Enter a descriptive title for the workflow"
- Apply proper `id`, `name`, and `maxlength` attributes
- Bind to component property for state management
- Use appropriate input validation attributes

## 5. Implement Body Textarea

Create a body textarea component:
- Use HTML `<textarea>` element
- Add readable label: "Workflow Description"
- Include helpful placeholder: "Describe the workflow requirements and implementation details..."
- Set appropriate `rows` (minimum 8) and `cols` attributes for usability
- Apply proper `id` and `name` attributes
- Bind to component property for state management
- Include character count display for user feedback

## 6. Update Navigation

Update **AdminPanel/Components/Layout/NavMenu.razor** to include the new admin page:
- Add navigation link to "/admin/duca"
- Use appropriate icon (suggest admin/settings icon)
- Follow existing navigation item patterns and styling
- Include readable display text: "DUCA Workflows"
- Position appropriately within the existing navigation structure

## 7. Add Component State Management

Implement proper component state management:
- Create properties for: `SelectedProject`, `WorkflowTitle`, `WorkflowBody`
- Initialize with appropriate default values
- Implement basic validation for required fields
- Add form submission placeholder method for future enhancement
- Include proper data binding with `@bind` directives

## Implementation Details

### Component Structure
- **File Location**: `AdminPanel/Components/Pages/Admin/Duca.razor`
- **Route**: `/admin/duca`
- **Render Mode**: Interactive Server for real-time updates
- **Layout**: Use default MainLayout from existing pages

### Form Field Specifications
- **Project Dropdown**: 
  - Options: "Addon", "AdminPanel", "Data"
  - Default: Placeholder option
  - Required field validation
- **Title Input**:
  - Type: text
  - Max length: 200 characters
  - Required field validation
- **Body Textarea**:
  - Minimum 8 rows for visibility
  - Character counter for user feedback
  - Required field validation

### Styling Guidelines
- Follow existing AdminPanel component styling patterns
- Use semantic HTML elements with proper labels
- Maintain consistency with existing form elements
- Ensure accessibility with proper label associations
- Include visual feedback for validation states

### Navigation Integration
- Add menu item in NavMenu.razor following existing patterns
- Use appropriate icon and positioning
- Ensure navigation link is highlighted when active
- Maintain existing navigation styling and behavior

### Success Criteria
- New `/admin/duca` route accessible via navigation
- All form fields render with proper labels and placeholders
- Project dropdown contains all three major projects
- Form state management works correctly with data binding
- Component follows existing AdminPanel styling patterns
- Navigation properly highlights active page
- All HTML elements use semantic markup with proper accessibility

### Out of Scope
- Do not implement actual workflow creation logic
- Do not add database persistence for workflows
- Do not implement form submission handling beyond placeholder
- Do not modify existing AdminPanel authentication or routing infrastructure
- Do not add external dependencies or styling frameworks
- Do not implement workflow validation beyond basic required field checks