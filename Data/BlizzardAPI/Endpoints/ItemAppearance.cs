using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

[Table("item_appearance", Schema = "wow")]
public sealed class ItemAppearance
{
    [Key]
    public int Id { get; set; }
    public SlotType SlotType { get; set; } = SlotType.UNKNOWN;
    public ClassType ClassType { get; set; } = ClassType.UNKNOWN;
    public SubclassType SubclassType { get; set; } = SubclassType.UNKNOWN;
    public int DisplayInfoId { get; set; }
    [MaxLength(511)] public string ItemIds { get; set; } = string.Empty;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
