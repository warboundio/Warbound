using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.ETL;

[Table("github_issues", Schema = "application")]
public class GitHubIssue
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public int IssueId { get; set; } = -1;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(1023)]
    public string Name { get; set; } = string.Empty;

    public bool WaitingForYou { get; set; }
}
