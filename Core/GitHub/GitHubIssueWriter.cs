using System.Text;

namespace Core.GitHub;

public class GitHubIssueWriter
{
    private readonly string SelectedProject = string.Empty;
    private readonly string WorkflowTitle = string.Empty;
    private readonly string WorkflowBody = string.Empty;
    private readonly bool IsPlanRequired;

    public GitHubIssueWriter(string selectedProject, string workflowTitle, string workflowBody, bool isPlanRequired)
    {
        SelectedProject = selectedProject;
        WorkflowTitle = workflowTitle;
        WorkflowBody = workflowBody;
        IsPlanRequired = isPlanRequired;
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.AppendLine($"This issue is created by the DUCA framework. Please review DUCA.md in the project root to understand the operating principles of this framework.");

        int numberOfSteps = 1;
        if (IsPlanRequired) { numberOfSteps++; }


        int stepNumber = 1;
        sb.AppendLine($"There are {numberOfSteps} steps to complete this issue:");
        if (IsPlanRequired)
        {
            sb.AppendLine($"# Step {stepNumber} | **Plan**: Create a plan for the issue in the {SelectedProject} project.");
            sb.AppendLine($"Your first pull request should only consist of this plan. After you submit this pull request a developer will review it and make edits.");
            sb.AppendLine($"If you see 'requested changes' that means that the developer has approved your plan and is ready for implementation. Please review the developers comments to confirm.");
            sb.AppendLine($"Ensure your plan speaks to every required change and (if necessary) it's purpose. You will not be able to extend your scope after we move away from this step.");
            sb.AppendLine($"Once you have moved onto the implementation stage the plan is immutable.");
            sb.AppendLine($"Please Review Core/Plans/PlanTemplate.md to understand the structure of the plan.");
            sb.AppendLine("");
            stepNumber++;
        }

        sb.AppendLine($"# Step {stepNumber} | **Implementation**");
        sb.AppendLine($"Do not scope creep outside of the approved upon plan.");
        sb.AppendLine($"It is during the implementation step that you are to update the documentation for any classes, namespaces, and projects to reflect current state.");

        sb.AppendLine("");
        sb.AppendLine($"# Developer Request | {WorkflowTitle}");
        sb.AppendLine($"The developer has provided these requirements for this issue:");
        sb.AppendLine($"> {WorkflowBody}");
        sb.AppendLine($"");
        sb.AppendLine($"If the developer has explicitly asked you to make changes that are normally not allowed in DUCA it is permissable to execute. Otherwise stay within the framework and coding standards that have been set.");

        if (IsPlanRequired) { sb.AppendLine($"Let's begin with the plan. Please create a PR that documents required changes to the codebase to implement the developer's request."); }
        if (!IsPlanRequired) { sb.AppendLine($"Let's begin with the implementation. Please create a PR that implements the developer's request."); }

        if (WorkflowTitle.Contains("Draft")) { sb.AppendLine($"Please review the appropriate {SelectedProject}/DraftsX.md files and look for a draft that matches this github issue description and title: '{WorkflowTitle}'. Please remove that draft as it is no longer needed."); }

        return sb.ToString();
    }
}
