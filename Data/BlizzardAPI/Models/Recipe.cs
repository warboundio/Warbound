using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;
using Data.Serialization;

namespace Data.BlizzardAPI.Models;

[Table("recipe", Schema = "wow")]
public sealed class Recipe
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [EncodedField(3)]
    public int Id { get; set; } = -1;

    [MaxLength(127)]
    public string Name { get; set; } = string.Empty;

    [EncodedField(2)]
    public int ProfessionId { get; set; } = -1;

    public int SkillTierId { get; set; } = -1;
    public int CraftedItemId { get; set; } = -1;
    public int CraftedQuantity { get; set; } = -1;
    [MaxLength(2047)] public string Reagents { get; set; } = string.Empty;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
