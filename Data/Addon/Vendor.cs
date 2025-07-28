using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Serialization;

namespace Data.Addon;

[Table("g_vendor", Schema = "wow")]
public class Vendor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [EncodedField(3)]
    public int NpcId { get; set; }

    [MaxLength(127)]
    [EncodedField]
    public string Name { get; set; } = string.Empty;

    [MaxLength(31)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(15)]
    [EncodedField]
    public string Faction { get; set; } = string.Empty;

    [EncodedField(2)]
    public int MapId { get; set; }

    [EncodedField(2)]
    public int X { get; set; }

    [EncodedField(2)]
    public int Y { get; set; }
}
