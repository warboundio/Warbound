using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ETL.BlizzardAPI.Enums;

namespace ETL.BlizzardAPI.Endpoints;

[Table("toy", Schema = "wow")]
public sealed class Toy
{
    [Key]
    public int Id { get; set; }
    [MaxLength(127)] public string Name { get; set; } = string.Empty;
    [MaxLength(31)] public string SourceType { get; set; } = string.Empty;
    public int MediaId { get; set; } = -1;
    public ETLStateType Status { get; set; } = ETLStateType.NEEDS_ENRICHED;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
