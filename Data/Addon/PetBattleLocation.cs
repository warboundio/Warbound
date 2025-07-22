using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Addon;

[Table("g_pet_battle_location", Schema = "wow")]
public class PetBattleLocation
{
    [Key]
    public int Id { get; set; }
    public int MapId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int PetSpeciesId { get; set; }
}
