using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Addon;

[Table("g_loot_log_entry", Schema = "wow")]
public class LootLogEntry
{
    [Key]
    public Guid Id { get; set; }
    public int NpcId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public int ZoneId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public DateTime CreatedAt { get; set; }
}
