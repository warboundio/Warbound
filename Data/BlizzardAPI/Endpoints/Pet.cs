using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;
using Data.Serialization;

namespace Data.BlizzardAPI.Endpoints;

[Table("pet", Schema = "wow")]
public sealed class Pet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [EncodedField(3)]
    public int Id { get; set; }

    [MaxLength(127)]
    [EncodedField]
    public string Name { get; set; } = string.Empty;

    [MaxLength(127)]
    [EncodedField]
    public string BattlePetType { get; set; } = string.Empty;

    [EncodedField]
    public bool IsCapturable { get; set; }

    [EncodedField]
    public bool IsTradable { get; set; }

    [EncodedField]
    public bool IsBattlePet { get; set; }

    [EncodedField]
    public bool IsAllianceOnly { get; set; }

    [EncodedField]
    public bool IsHordeOnly { get; set; }

    [MaxLength(127)] public string SourceType { get; set; } = string.Empty;
    [MaxLength(255)] public string Icon { get; set; } = string.Empty;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
