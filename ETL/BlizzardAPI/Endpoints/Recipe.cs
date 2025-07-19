using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ETL.BlizzardAPI.Enums;

namespace ETL.BlizzardAPI.Endpoints;

[Table("recipe", Schema = "wow")]
public sealed class Recipe
{
    [Key]
    public int Id { get; set; } = -1;
    [MaxLength(127)] public string Name { get; set; } = string.Empty;
    public int ProfessionId { get; set; } = -1;
    public int SkillTierId { get; set; } = -1;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}