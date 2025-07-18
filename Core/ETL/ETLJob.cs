using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.ETL;

[Table("etl_jobs", Schema = "application")]
public class ETLJob
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(1023)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(63)]
    public string CRONSchedule { get; set; } = string.Empty;

    public DateTime? LastSuccessAt { get; set; }

    [MaxLength(127)]
    public string? LockOwner { get; set; }
    public DateTime? LockAcquiredAt { get; set; }
    public int LockTimeoutMinutes { get; set; } = 60;
    public bool WasLastRunSuccessful { get; set; }
    public int? LastDurationMilliseconds { get; set; }
}
