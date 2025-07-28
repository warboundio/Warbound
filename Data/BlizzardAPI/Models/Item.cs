using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.BlizzardAPI.Enums;
using Data.Serialization;

namespace Data.BlizzardAPI.Models;

[Table("item", Schema = "wow")]
public sealed class Item
{
    [Key]
    [EncodedField(3)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [EncodedField]
    [MaxLength(127)]
    public string Name { get; set; } = string.Empty;

    [EncodedField(3)]
    public QualityType QualityType { get; set; } = QualityType.UNKNOWN;

    public int Level { get; set; }

    public int RequiredLevel { get; set; }

    [EncodedField]
    [MaxLength(63)]
    public string ClassType { get; set; } = "UNKNOWN";

    [EncodedField]
    public SubclassType SubclassType { get; set; }

    [EncodedField]
    [MaxLength(127)]
    public string InventoryType { get; set; } = string.Empty;

    public int PurchasePrice { get; set; }
    public int SellPrice { get; set; }
    public int MaxCount { get; set; }
    public bool IsEquippable { get; set; }
    public bool IsStackable { get; set; }
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
