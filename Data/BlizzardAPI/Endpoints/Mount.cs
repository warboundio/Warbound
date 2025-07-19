using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

[Table("mount", Schema = "wow")]
public sealed class Mount
{
    [Key]
    public int Id { get; set; }
    [MaxLength(127)] public string Name { get; set; } = string.Empty;
    [MaxLength(63)] public string SourceType { get; set; } = string.Empty;
    public int CreatureDisplayId { get; set; } = -1;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
