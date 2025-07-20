using Core.Models;
using System.Text.RegularExpressions;

namespace Core.Services;

public partial class DraftService
{
    private readonly List<DraftItem> _draftItems = new();
    private readonly string _solutionPath;

    [GeneratedRegex(@"##\s*Draft:\s*(.+?)\s*\n###\s*Agent\s*\n(.+?)(?=\n##|\n###|\Z)", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex DraftPattern();

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
        
        foreach (string file in markdownFiles)
        {
            ParseDraftsFromFile(file);
        }
    }

    private void ParseDraftsFromFile(string filePath)
    {
        string projectName = GetProjectNameFromPath(filePath);
        if (string.IsNullOrEmpty(projectName))
        {
            return;
        }

        string content = File.ReadAllText(filePath);
        
        // Pattern to match: ## Draft: [Title] followed by ### Agent and then the next line as text
        MatchCollection matches = DraftPattern().Matches(content);

        foreach (Match match in matches)
        {
            string title = match.Groups[1].Value.Trim();
            string text = match.Groups[2].Value.Trim();
            
            _draftItems.Add(new DraftItem
            {
                ProjectName = projectName,
                Title = title,
                Text = text
            });
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
            if (firstDirectory == "AdminPanel" || firstDirectory == "Addon" || firstDirectory == "Data")
            {
                return firstDirectory;
            }
        }
        
        return string.Empty;
    }

    public List<DraftItem> GetDraftsByProject(string projectName)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            return new List<DraftItem>();
        }
        
        return _draftItems.Where(d => d.ProjectName.Equals(projectName, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<DraftItem> GetAllDrafts()
    {
        return new List<DraftItem>(_draftItems);
    }

    public void RefreshDrafts()
    {
        LoadDrafts();
    }
}