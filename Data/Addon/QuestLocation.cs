using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Addon;

[Table("g_quest_location", Schema = "wow")]
public class QuestLocation
{
    [Key]
    [Column(Order = 0)]
    public int QuestId { get; set; }

    [Key]
    [Column(Order = 1)]
    public bool IsStart { get; set; }

    [MaxLength(63)]
    public string NpcName { get; set; } = string.Empty;

    public int FactionId { get; set; }

    public int MapId { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public int NpcId { get; set; }
}
