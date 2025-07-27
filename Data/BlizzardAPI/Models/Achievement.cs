using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;
using Data.Serialization;

namespace Data.BlizzardAPI.Models;

[Table("achievement", Schema = "wow")]
public sealed class Achievement
{
    [Key]
    [EncodedField(3)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [MaxLength(255)]
    [EncodedField]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2047)] public string Description { get; set; } = string.Empty;
    [MaxLength(255)] public string CategoryName { get; set; } = string.Empty;
    [MaxLength(2047)] public string RewardDescription { get; set; } = string.Empty;
    public int? RewardItemId { get; set; }
    [MaxLength(255)] public string RewardItemName { get; set; } = string.Empty;
    public int Points { get; set; }
    [MaxLength(255)] public string Icon { get; set; } = string.Empty;
    [MaxLength(2047)] public string CriteriaIds { get; set; } = string.Empty;
    [MaxLength(2047)] public string CriteriaTypes { get; set; } = string.Empty;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
