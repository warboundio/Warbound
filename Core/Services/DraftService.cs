using Core.Models;

namespace Core.Services;

public partial class DraftService
{
    private List<DraftItem> _draftItems = [];
    private readonly string _solutionPath;

    public DraftService()
    {
        _solutionPath = GetSolutionPath();
        LoadDrafts();
    }

    private string GetSolutionPath()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        DirectoryInfo? directory = new(currentDirectory);

        while (directory != null)
        {
            if (directory.GetFiles("*.sln").Length > 0)
            {
                return directory.FullName;
            }
            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not find solution file in directory hierarchy");
    }

    private void LoadDrafts()
    {
        _draftItems.Clear();

        string[] markdownFiles = Directory.GetFiles(_solutionPath, "*.md", SearchOption.AllDirectories);
        foreach (string file in markdownFiles) { ParseDraftsFromFile(file); }
    }

    private void ParseDraftsFromFile(string filePath)
    {
        string projectName = GetProjectNameFromPath(filePath);
        if (string.IsNullOrEmpty(projectName))
        {
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].TrimStart().StartsWith("## Draft:", StringComparison.OrdinalIgnoreCase))
            {
                string title = lines[i][(lines[i].IndexOf(':') + 1)..].Trim();

                int agentLine = i + 1;
                while (agentLine < lines.Length && string.IsNullOrWhiteSpace(lines[agentLine])) { agentLine++; }

                if (agentLine < lines.Length && lines[agentLine].TrimStart().StartsWith("## Agent", StringComparison.OrdinalIgnoreCase))
                {
                    int textLine = agentLine + 1;
                    while (textLine < lines.Length && string.IsNullOrWhiteSpace(lines[textLine])) { textLine++; }

                    if (textLine < lines.Length)
                    {
                        string text = lines[textLine].Trim();
                        _draftItems.Add(new DraftItem
                        {
                            ProjectName = projectName,
                            Title = title,
                            Text = text
                        });
                    }
                }
            }
        }
    }

    private string GetProjectNameFromPath(string filePath)
    {
        string relativePath = Path.GetRelativePath(_solutionPath, filePath);
        string[] pathParts = relativePath.Split(Path.DirectorySeparatorChar);

        if (pathParts.Length > 0)
        {
            string firstDirectory = pathParts[0];
            // Only return known project names
            if (firstDirectory is "AdminPanel" or "Addon" or "Data")
            {
                return firstDirectory;
            }
        }

        return string.Empty;
    }

    public List<DraftItem> GetDraftsByProject(string projectName)
    {
        return string.IsNullOrEmpty(projectName)
            ? []
            : [.. _draftItems.Where(d => d.ProjectName.Equals(projectName, StringComparison.OrdinalIgnoreCase))];
    }

    public void RemoveDraft(string title) => _draftItems = [.. _draftItems.Where(o => o.Title != title)];
    public List<DraftItem> GetAllDrafts() => [.. _draftItems];

    public void RefreshDrafts() => LoadDrafts();
}
