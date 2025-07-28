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

    [MaxLength(63)]
    [EncodedField]
    public string SlotType { get; set; } = string.Empty;

    [EncodedField]
    [MaxLength(63)]
    public string ClassType { get; set; } = "UNKNOWN";

    [EncodedField]
    [MaxLength(63)]
    public string SubclassType { get; set; } = string.Empty;

    public int DisplayInfoId { get; set; }

    [MaxLength(511)] public string ItemIds { get; set; } = string.Empty;

    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;

    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
