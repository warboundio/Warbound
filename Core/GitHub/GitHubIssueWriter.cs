using System.Text;

namespace Core.GitHub;

public class GitHubIssueWriter
{
    private readonly string SelectedProject = string.Empty;
    private readonly string WorkflowBody = string.Empty;
    private readonly bool IsPlanRequired;

    public GitHubIssueWriter(string selectedProject, string workflowBody, bool isPlanRequired)
    {
        SelectedProject = selectedProject;
        WorkflowBody = workflowBody;
        IsPlanRequired = isPlanRequired;
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        int stepNumber = 1;
        sb.AppendLine($"In order to complete this issue you must follow these steps in order.");
        if (IsPlanRequired)
        {
            sb.AppendLine($"# Step {stepNumber} | Create a PLAN for the issue in the {SelectedProject} project.");
            sb.AppendLine($"- Your first pull request should only consist of this Plan.md file (and possibly removing the draft). Review Core/Plans/PlanTemplate.md for how to create a Plan, placing the file in the Plans folder of the primary project. Follow the rules of your copilot-instructions.md regarding plans.");
            sb.AppendLine($"- Since you are being asked to create a plan, it's likely that there is a corresponding 'Draft' in one of the project 'Draft.md' files. Please remove it as we not longer need to track that work.");
            sb.AppendLine("");
            stepNumber++;
        }

        sb.AppendLine($"# Step {stepNumber} | Implementation");
        if (IsPlanRequired) { sb.AppendLine($"- The developer will 'Request Changes' to your PR to advance to Step 2 and ask you to start executing or implementing. Only then can you begin on this step."); }
        if (IsPlanRequired) { sb.AppendLine($"- Please recheck the plan document you wrote. It may have been adjusted by the developer. The plan is now immutable. Do not scope creep or make unapproved changes."); }
        sb.AppendLine($"- It is in this step that you may begin writing code. After you have completed all of the required changes you may move onto Step {stepNumber + 1}.");
        sb.AppendLine("");
        stepNumber++;

        sb.AppendLine($"# Step {stepNumber} | Coding Conventions");
        sb.AppendLine($"- You must follow all coding conventions found at /.github/copilot-coding-conventions.md.");
        sb.AppendLine($"- If the code does not meet these standards your pull request will be sent back and asked to fix it. This costs GitHub the company extra money and is a waste of resources.");
        sb.AppendLine("");
        stepNumber++;

        sb.AppendLine($"# Step {stepNumber} | Final Review");
        sb.AppendLine($"- Do one final pass through the code. Ensure we are following our coding philosophies found in .github/copilot-instructions, the coding standards in .github/copilot-coding-conventions.md, and the coding meets the minimum requirements to complete the developers request.");
        sb.AppendLine("");
        stepNumber++;

        sb.AppendLine($"# Step {stepNumber} | Pull Request");
        sb.AppendLine($"- You've made it to the end! Great job! Let's put in that pull request. If changes are requested please pay careful attentions to the developer notes.");
        sb.AppendLine("");

        sb.AppendLine($"# Project | {SelectedProject}");
        sb.AppendLine($"- This issue is part of the {SelectedProject} project. Please ensure you are following the project guidelines and structure.");

        sb.AppendLine($"- Developer Request");
        sb.AppendLine($"The developer has provided these requirements for this issue. Speech to text is used.");
        sb.AppendLine($"> {WorkflowBody}");
        sb.AppendLine($"");

        if (IsPlanRequired) { sb.AppendLine($"Let's begin with the STEP 1: Creating a Plan. Please review Core/Plans/PlanTemplate.md to understand the structure of how to create a plan. Create a PR that documents required changes to the codebase to implement the developer's request."); }
        if (!IsPlanRequired) { sb.AppendLine($"Let's begin with STEP 1: Implementation. Please ensure ALL steps are followed carefully before asking a developer to review."); }

        return sb.ToString();
    }
}
