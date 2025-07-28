using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.BlizzardAPI.Models;

[Table("object_expansion", Schema = "wow")]
public sealed class ObjectExpansionMapping
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public char CollectionType { get; set; } // 'P' for Pet, 'T' for Toy, 'M' for Mount, 'A' for Appearance, 'R' for Recipe
    public int ExpansionId { get; set; }
}
