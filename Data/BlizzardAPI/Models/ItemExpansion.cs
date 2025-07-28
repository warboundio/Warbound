using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.BlizzardAPI.Models;

[Table("item_expansion", Schema = "wow")]
public sealed class ItemExpansion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ItemId { get; set; }
    public int ExpansionId { get; set; }
}
