using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;
using Data.Serialization;

namespace Data.BlizzardAPI.Models;

[Table("mount", Schema = "wow")]
public sealed class Mount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [EncodedField(3)]
    public int Id { get; set; }

    [MaxLength(127)]
    [EncodedField]
    public string Name { get; set; } = string.Empty;

    [MaxLength(63)]
    public string SourceType { get; set; } = string.Empty;

    public int ItemId { get; set; }
    public int CreatureDisplayId { get; set; } = -1;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
