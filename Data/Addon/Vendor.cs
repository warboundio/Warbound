using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Addon;

[Table("g_vendor", Schema = "wow")]
public class Vendor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int NpcId { get; set; }
    [MaxLength(127)] public string Name { get; set; } = string.Empty;
    [MaxLength(31)] public string Type { get; set; } = string.Empty;
    [MaxLength(15)] public string Faction { get; set; } = string.Empty;
    public int MapId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}
