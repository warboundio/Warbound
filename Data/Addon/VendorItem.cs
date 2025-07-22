using System.ComponentModel.DataAnnotations;

namespace Data.Addon;

public class VendorItem
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public int Cost { get; set; }
    [MaxLength(7)] public string CostType { get; set; } = string.Empty;
    public int CostId { get; set; }
    public int VendorId { get; set; }
}
