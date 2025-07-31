using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Addon;

[Table("g_npc_kill_count", Schema = "wow")]
public class NpcKillCount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int NpcId { get; set; }
    [MaxLength(127)] public string Name { get; set; } = "UNKNOWN";
    public int Count { get; set; }
}
