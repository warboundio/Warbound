using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Addon;

[Table("g_loot_item_summary", Schema = "wow")]
public class LootItemSummary
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int NpcId { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ItemId { get; set; }

    public int Quantity { get; set; }
}
