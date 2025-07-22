using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Addon;

[Table("g_vendor_item", Schema = "wow")]
public class VendorItem
{
    public int NPCId { get; set; }
    public int VendorId { get; set; }
    public int Quantity { get; set; }
    public int Cost { get; set; }
    [MaxLength(7)] public string CostType { get; set; } = string.Empty;
    public int CostId { get; set; }
}
