using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

[Table("pet", Schema = "wow")]
public sealed class Pet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(127)] public string Name { get; set; } = string.Empty;
    [MaxLength(127)] public string BattlePetType { get; set; } = string.Empty;
    public bool IsCapturable { get; set; }
    public bool IsTradable { get; set; }
    public bool IsBattlePet { get; set; }
    public bool IsAllianceOnly { get; set; }
    public bool IsHordeOnly { get; set; }
    [MaxLength(127)] public string SourceType { get; set; } = string.Empty;
    [MaxLength(255)] public string Icon { get; set; } = string.Empty;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
