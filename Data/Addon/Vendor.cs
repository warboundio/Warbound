namespace Data.Addon;

public class Vendor
{
    public int NpcId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Faction { get; set; } = string.Empty;
    public int MapId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}
