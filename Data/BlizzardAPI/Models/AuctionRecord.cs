using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.BlizzardAPI.Models;

[Table("auction", Schema = "wow")]
public class AuctionRecord
{
    public Guid Id { get; set; }
    public int ItemId { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsCommodity { get; set; }
    public long Price { get; set; }
    public int Quantity { get; set; }
}
