namespace Core.GitHub;

/// <summary>
/// Interface for creating GitHub issues programmatically
/// </summary>
public interface IGitHubIssueService
{
    /// <summary>
    /// Creates a GitHub issue with specified title and body, and attaches it to the project
    /// </summary>
    /// <param name="title">Issue title</param>
    /// <param name="body">Issue body content</param>
    /// <returns>GitHub issue number</returns>
    Task<int> Create(string title, string body);
}