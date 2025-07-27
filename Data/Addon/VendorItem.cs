using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Serialization;

namespace Data.Addon;

[Table("g_vendor_item", Schema = "wow")]
public class VendorItem
{
    public int ItemId { get; set; }
    public int VendorId { get; set; }
    public int Quantity { get; set; }

    [EncodedField(3)]
    public int Cost { get; set; }

    [EncodedField]
    [MaxLength(7)]
    public string CostType { get; set; } = string.Empty;

    [EncodedField(1)]
    public int CostId { get; set; }
}
