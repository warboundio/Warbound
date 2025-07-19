using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

[Table("profession", Schema = "wow")]
public sealed class Profession
{
    [Key]
    public int Id { get; set; } = -1;
    [MaxLength(127)] public string Name { get; set; } = string.Empty;
    [MaxLength(127)] public string Type { get; set; } = string.Empty;
    [MaxLength(1027)] public string SkillTiers { get; set; } = string.Empty;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}