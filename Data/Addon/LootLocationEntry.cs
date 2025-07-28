using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Addon;

[Table("g_loot_location_entry", Schema = "wow")]
public class LootLocationEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int NpcId { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int X { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Y { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ZoneId { get; set; }
}
