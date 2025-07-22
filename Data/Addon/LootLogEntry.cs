using System;

namespace Data.Addon;

public class LootLogEntry
{
    public Guid Id { get; set; }
    public int NpcId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public int ZoneId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public DateTime CreatedAt { get; set; }
}
