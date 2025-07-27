using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;
using Data.Serialization;

namespace Data.BlizzardAPI.Models;

[Table("item_appearance", Schema = "wow")]
public sealed class ItemAppearance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [EncodedField(3)]
    public int Id { get; set; }

    [EncodedField]
    public SlotType SlotType { get; set; } = SlotType.UNKNOWN;

    [EncodedField]
    public ClassType ClassType { get; set; } = ClassType.UNKNOWN;

    [EncodedField]
    public SubclassType SubclassType { get; set; } = SubclassType.UNKNOWN;

    public int DisplayInfoId { get; set; }

    [MaxLength(511)] public string ItemIds { get; set; } = string.Empty;

    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;

    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
